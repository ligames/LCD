using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class GunAnimator
{
    GunTemplate template;
    public GunTemplate Template
    {
        get
        {
            return template;
        }
        set
        {
            template = value;
            int type = template == null ? 0 : template.weaponType;
            Animator.SetInteger("WeaponType_int", type);
        }
    }
    public Animator Animator { get; set; }
    public void BeginShootAnimation()
    {
        Animator.SetBool("Shoot_b", true);
    }
    public void Stop()
    {
        Animator.SetBool("Shoot_b", false);
    }
    public bool IsIdle { get { return Animator.GetCurrentAnimatorStateInfo(5).IsName("Character_" + template.idleAnimation); } }
    public bool IsShooting { get { return Animator.GetCurrentAnimatorStateInfo(5).IsName("Character_" + template.singleShotAnimation); } }
}

public class Person : MonoBehaviour
{
    public uint radiusInCells = 6;
    PersonActionManager manager;
    protected PersonActionManager Manager
    {
        get
        {
            if (!manager)
                manager = FindObjectOfType<PersonActionManager>();
            return manager;
        }
        set
        {
            manager = value;
        }
    }
    Animator animator;
    protected Animator Animator
    {
        get
        {
            if (!animator)
                animator = GetComponentInChildren(typeof(Animator), true) as Animator;
            return animator;
        }
    }
    public float rotationSpeed = 2.0f;
    public float health = 3;
    //public BackpackContents Backpack { get { return manager.BackPack; } }
    public bool Alive { get; protected set; }
    protected float currentHealth;
    protected float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            SetHealth(value);
        }
    }
    public ParticleSystem bloodSplatter;
    protected NavMeshAgent navigationAgent;
    public float receivingDamageTime = 0.5f;
    public HealthBar healthBar;
    public float eatingTime = 1.2f;
    public int eatingAnimationNumber = 5;

    protected void Awake()
    {
        Manager = FindObjectOfType<PersonActionManager>();
        Alive = true;
        navigationAgent = GetComponent<NavMeshAgent>();
        Manager.BindPersonToField(GetComponent<Person>());
        bloodSplatter.Stop();
    }
    protected void Start()
    {
        healthBar.MaxHealth = health;
        CurrentHealth = health;
    }
    protected virtual void SetHealth(float health)
    {
        if (health > 0)
        {
            currentHealth = health;
            healthBar.Health = currentHealth;
        }
        else
            StartCoroutine(Die());
    }
    // ==============================================================
    #region API
    public virtual IEnumerator ReceiveDamage(float damage)
    {
        CurrentHealth -= damage;
        bloodSplatter.Play();
        yield return null;
    }
    public float DistanceToCell(FieldCell cell)
    {
        NavMeshPath path = new NavMeshPath();
        navigationAgent.CalculatePath(cell.transform.position, path);
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = cell.transform.position;
        for (int i = 0; i < path.corners.Length; i++)
            allWayPoints[i + 1] = path.corners[i];
        float pathLength = 0;
        for (int i = 0; i < allWayPoints.Length - 1; i++)
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        return pathLength;
    }
    public IEnumerator MoveTowards(FieldCell cell)
    {
        Animator.SetFloat("Speed_f", 0.6f);
        navigationAgent.isStopped = false;

        Vector3 destination = cell.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, Manager.GameInfo.radiusError, NavMesh.AllAreas))
        {
            navigationAgent.SetDestination(hit.position);
            while ((transform.position - destination).magnitude > Manager.GameInfo.radiusError)
                yield return null;
        }
        else
        {
            navigationAgent.SetDestination(destination);
            while ((transform.position - destination).magnitude > Manager.GameInfo.radiusError)
                yield return null;
        }
        Stop();
        Manager.PersonPositionChanged(GetComponent<Person>());
        OnMoved();
    }
    public void Stop()
    {
        Animator.SetFloat("Speed_f", 0.0f);
        navigationAgent.isStopped = true;
    }
    public IEnumerator LookAtCell(FieldCell cell)
    {
        Vector3 destination = cell.transform.position;
        Quaternion destRotation = Quaternion.LookRotation(destination - transform.position);
        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destRotation, Time.deltaTime * rotationSpeed);
            if ((transform.rotation.eulerAngles - destRotation.eulerAngles).magnitude <= Manager.GameInfo.rotationError)
                break;
            yield return null;
        }
    }
    public void ShowBar()
    {
        healthBar.gameObject.SetActive(true);
    }
    public void HideBar()
    {
        healthBar.gameObject.SetActive(false);
    }
    #endregion
    // ==============================================================

    protected virtual IEnumerator Die()
    {
        foreach (Collider collider in GetComponentsInChildren(typeof(Collider)))
            Destroy(collider);
        Destroy(navigationAgent);
        yield return new WaitForSeconds(1.0f);
        SetDefaultLayer(transform, 0);
    }
    void SetDefaultLayer(Transform t, int layer)
    {
        for(int i = 0; i < t.childCount; ++i)
            SetDefaultLayer(t.GetChild(i), layer);
        t.gameObject.layer = layer;
    }
    protected virtual void OnMoved() { }
}

[System.Serializable]
public class StepInfo
{
    PersonActionController person;

    public int movesPerStep = 1;
    public Field Field { get; set; }
    public bool HasShot { get; set; }
    public int Moves { get; private set; }

    [Inject]
    BackpackContents backPack;

    
    public PersonActionController Person
    {
        get
        {
            return person;
        }
        set
        {
            person = value;
            person.onMoved += () => --Moves;
            person.onShot += () => HasShot = true;
        }
    }

    public List<ZombieController> Aims { get; private set; }
    public List<FieldCell> CellsToMove { get; private set; }
    
    public bool CanMove
    {
        get
        {
            return !HasShot && Moves > 0;
        }
    }
    public bool CanFire
    {
        get
        {
            if (HasShot || !Person.IsArmored)
                return false;
            return Aims.Count > 0;
        }
    }
    public bool CanUse
    {
        get
        {
            return backPack.ContainExpendable;
        }
    }
    public bool Over
    {
        get { return HasShot || !CanMove && !CanFire && !Person.IsInCarArea; }
    }

    public StepInfo()
    {
        Aims = new List<ZombieController>();
        CellsToMove = new List<FieldCell>();
    }
    
    public void StartStep()
    {
        Moves = movesPerStep;
        HasShot = false;
    }
    public void CalculateMoves()
    {
        CellsToMove = Field.NearestCells(Person);
    }
    public void CalculateAims(ZombieController[] zombies)
    {
        Aims.Clear();
        float raduis = Person.radiusInCells * Field.CellSize;
        foreach (ZombieController zombie in zombies)
        {
            if (zombie.Alive && Vector3.Distance(Person.transform.position, zombie.transform.position) <= raduis)
                Aims.Add(zombie);
        }
    }
}

public class PersonActionController : Person
{
    public GunResourceInfo Gun { get; private set; }
    public StepInfo stepInfo;
    public GameObject muzzleEffectPrefab;
    public GameObject cameraMountsContainer;
    public event System.Action onMoved;
    public event System.Action onShot;
    public bool IsInCarArea { get; set; }
    PersonState state;
    public PersonState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            if (state.GunId >= 0)
                Adopt(GlobalInfo.GunsResources[state.GunId]);
        }
    }

    Transform muzzleEffectMount;
    Transform gunMount;
    Transform GunMount
    {
        get
        {
            if (!gunMount)
                gunMount = GameInfo.GetObjectByName(transform, "Hand_Right_jnt").transform;
            return gunMount;
        }
        set
        {
            gunMount = value;
        }
    }
    GunAnimator gunAnimator;
    GunAnimator GunAnimator
    {
        get
        {
            if (gunAnimator == null)
                gunAnimator = new GunAnimator { Animator = Animator };
            return gunAnimator;
        }
    }

    public bool IsArmored { get { return Gun != null; } }
    List<Transform> cameraMounts;

    BackpackContents backPack;
    GlobalInfoAccessor globalInfo;
    GlobalInfoAccessor GlobalInfo
    {
        get
        {
            if (!globalInfo)
                globalInfo = FindObjectOfType<GlobalInfoAccessor>();
            return globalInfo;
        }
        set
        {
            globalInfo = value;
        }
    }

    Transform randomCameraMount
    {
        get
        {
            return cameraMounts[new System.Random().Next(cameraMounts.Count)];
        }
    }

    new void Awake()
    {
        base.Awake();
        backPack = FindObjectOfType<BackpackContents>();
        stepInfo.Field = Manager.Field;
        stepInfo.Person = GetComponent<PersonActionController>();
        IsInCarArea = true;
        
        GetCameraMounts();
    }
    new void Start()
    {
        base.Start();
        GunMount = GameInfo.GetObjectByName(transform, "Hand_Right_jnt").transform;
        Stop();
    }

    public override IEnumerator ReceiveDamage(float damage)
    {
        if (Alive)
        {
            Animator.SetBool("Jump_b", true);
            yield return new WaitForSeconds(receivingDamageTime);
            Animator.SetBool("Jump_b", false);
            yield return base.ReceiveDamage(damage);
        }
    }
    public IEnumerator Shoot(ZombieController receiver)
    {
        //manager.ShootingFilming(randomCameraMount);
        yield return new WaitForSeconds(0.35f);
        Vector3 destination = receiver.transform.position;

        yield return LookAtCell(Manager.Field.NearestCell(receiver.transform.position));
        float damg = Gun.constantDamage +
            Gun.distanceSensDamage /
            (transform.position - receiver.transform.position).magnitude;
        float shots = Mathf.Min(receiver.MaxShotsCanReceive(damg), 3);

        while (shots-- > 0)
        {
            GunAnimator.BeginShootAnimation();

            //while (!GunAnimator.IsShooting) //waiting for shot anim
            //    yield return null;
            yield return new WaitForEndOfFrame();

            GunAnimator.Stop();
            StartCoroutine(MuzzleEffectFlow());
            yield return receiver.ReceiveDamage(damg);
            yield return null;
        }
        yield return new WaitForSeconds(0.35f);
        if (onShot != null)
            onShot();
        Manager.StopShotFilming();
    }
    IEnumerator MuzzleEffectFlow()
    {
        GameObject muzzleEffect = Instantiate(muzzleEffectPrefab, muzzleEffectMount);
        yield return new WaitForSeconds(0.05f);
        Destroy(muzzleEffect);
    }
    public void Adopt(GunResourceInfo gun)
    {
        GunResourceInfo oldGun = GiveUpArms();
        GameObject gunGO = Instantiate(gun.cleanPrefab, GunMount);
        gunGO.transform.localPosition = Manager.GameInfo.gunPosition;
        gunGO.transform.Rotate(Manager.GameInfo.gunRotation);

        muzzleEffectMount = GameInfo.GetObjectByName(gunGO.transform, "Mount").transform;
        if (!muzzleEffectMount)
            muzzleEffectMount = gunGO.transform;

        Debug.Log(gun.template);
        GunAnimator.Template = gun.template;
        Gun = gun;
        State.GunId = GlobalInfo.GetGunId(Gun);
        Debug.Log("gun is: " + State.GunId);
        if (oldGun)
            backPack.AddItem(oldGun);
    }
    public GunResourceInfo GiveUpArms()
    {
        if (GunMount.childCount > 0)
            Destroy(GunMount.GetChild(0).gameObject);
        GunResourceInfo arms = Gun;
        Gun = null;
        if(gunAnimator != null)
            gunAnimator.Template = null;
        return arms;
    }
    public IEnumerator UseMedical()
    {
        backPack.UseMedical();
        //GunResourceInfo gun = GiveUpArms();
        GunTemplate template = gunAnimator.Template;
        gunAnimator.Template = null;
        Animator.SetInteger("Animation_int", eatingAnimationNumber);
        yield return new WaitForSeconds(eatingTime);
        CurrentHealth = Mathf.Min(CurrentHealth + GlobalInfo.MedicalRestoringValue, health);
        Animator.SetInteger("Animation_int", 0);
        gunAnimator.Template = template;
        //if (gun)
        //    Adopt(gun);
    }
    protected override void OnMoved()
    {
        if (onMoved != null)
            onMoved();
    }
    protected override IEnumerator Die()
    {
        if (Alive)
        {
            Alive = false;
            Manager.CharDied(GetComponent<PersonActionController>());
            Manager.CharRemoved(GetComponent<PersonActionController>());
            System.Random r = new System.Random();
            HideBar();
            GunAnimator.Stop();
            Animator.SetInteger("DeathType_int", r.Next(2) + 1);
            Animator.SetBool("Death_b", true);
            yield return base.Die();
        }
    }
    void GetCameraMounts()
    {
        cameraMounts = new List<Transform>();
        for (int i = 0; i < cameraMountsContainer.transform.childCount; ++i)
            cameraMounts.Add(cameraMountsContainer.transform.GetChild(i));
    }
}
