using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PersonStepManager : MonoBehaviour
{
    [Inject]
    GlobalInfo globalInfo;
    [SerializeField]
    Text hintText;
    [Inject]
    Field field;
    [SerializeField]
    Button nextCharBut, healBut, endStepBut, cancelBut;

    public Car car;

    public GameObject aimPrefab;
    List<GameObject> aims;
    GameInfo gameInfo;
    [SerializeField]
    Button fireButton;

    bool fire;
    public bool havePlayerMadeDecision { get; private set; }
    public event System.Action onStepOver;

    public bool Fire
    {
        get
        {
            return fire;
        }
        private set
        {
            if (currentPerson && !havePlayerMadeDecision)
            {
                if (value)
                {
                    if (currentPerson.stepInfo.CanFire)
                    {
                        SetActiveControlButtons(false);
                        fire = true;
                        hintText.text = ">>" + globalInfo.ShootingHint;
                        StopPersonActivityCoroutines();
                        field.DeactivateActiveCells();
                        StartCoroutine(DoFire());
                    }
                }
                else
                {
                    currentPerson.stepInfo.CalculateMoves();
                    if(currentPerson.stepInfo.CanMove)
                    {
                        SetActiveControlButtons(true);
                        fire = false;
                        hintText.text = ">>" + globalInfo.MovingHint;
                        StopPersonActivityCoroutines();
                        DeactivateAims();
                        StartCoroutine(DoMovement());
                    }
                }
                SetActiveZombiesBars(fire);
                StartCoroutine(LookAtStepFlow());
            }
        }
    }

    PersonActionController currentPerson;
    List<FieldCell> PersonPossibleMoves { get { return currentPerson.stepInfo.CellsToMove; } }
    List<ZombieController> personPossibleFires { get { return currentPerson.stepInfo.Aims; } }
    public bool IsStepOver { get; private set; }

    void Awake()
    {
        gameInfo = FindObjectOfType<GameInfo>();
        fireButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnSwitcherButtonClick));
        aims = new List<GameObject>();
        cancelBut.onClick.AddListener(new UnityEngine.Events.UnityAction(OnCancelClick));
        StartCoroutine(ShootButtonActivity());
    }

    void ActivateAims()
    {
        float height = personPossibleFires[0].GetComponent<Collider>().bounds.size.y / 2.0f;
        foreach (ZombieController zombie in personPossibleFires)
        {
            if (!zombie.Alive)
                continue;
            GameObject aim = Instantiate(aimPrefab);
            aim.transform.position = zombie.transform.position + zombie.transform.up * height;
            aims.Add(aim);
        }
    }
    void SetActiveZombiesBars(bool active)
    {
        foreach (ZombieController zombie in FindObjectsOfType<ZombieController>())
        {
            if (!zombie.Alive)
                continue;
            if (active)
                zombie.ShowBar();
            else
                zombie.HideBar();
        }
    }
    void DeactivateAims()
    {
        foreach (GameObject aim in aims)
            Destroy(aim);
        aims.Clear();
    }
    void OnSwitcherButtonClick()
    {
        Fire = !Fire;
    }

    RaycastHit[] GetUsefulRaycastHits()
    {
        RaycastHit[] hits = null;
        if (!car.IsTapped)
        {
            TapPosition tapPos = globalInfo.TapInput.GetTap();
            if (tapPos != null && !GameInfo.IsTapOnUI(tapPos.Position))
            {
                Ray ray = Camera.main.ScreenPointToRay(tapPos.Position);
                hits = Physics.RaycastAll(ray);
            }
        }
        return hits;
    }
    IEnumerator DoMovement()
    {
        field.ActivateNearestCells(currentPerson);
        while (!havePlayerMadeDecision)
        {
            RaycastHit[] hits;
            while ((hits = GetUsefulRaycastHits()) == null)
                yield return null;

            foreach (RaycastHit hit in hits)
                if (hit.collider.gameObject.tag == GameInfo.terrainTag)
                {
                    FieldCell tappedCell = field.GetActiveCellInPosition(hit.point);
                    if (tappedCell)
                    {
                        havePlayerMadeDecision = true;
                        Debug.Log("tap in rect");
                        field.DeactivateActiveCells();
                        yield return currentPerson.MoveTowards(tappedCell);
                        IsStepOver = true;
                        havePlayerMadeDecision = false;
                        CalculateCurrentPersonAims();
                        TryFire();
                        //SetGetInCarButton();
                    }
                    break;
                }
            yield return null;
        }
    }
    IEnumerator DoFire()
    {
        ActivateAims();
        while (!havePlayerMadeDecision)
        {
            RaycastHit[] hits;
            while ((hits = GetUsefulRaycastHits()) == null)
                yield return null;
            havePlayerMadeDecision = true;

            Vector3 tapPosition = new Vector3();
            ZombieController wantedZombie = null;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.name == GameInfo.terrainTag)
                    tapPosition = hit.collider.transform.position;
                if(wantedZombie = hit.collider.gameObject.GetComponent<ZombieController>())
                {
                    DeactivateAims();
                    yield return currentPerson.Shoot(wantedZombie);
                    IsStepOver = true;
                    break;
                }
            }
            if (!IsStepOver)
            {
                foreach (ZombieController zombie in personPossibleFires)
                    if (wantedZombie == null || zombie && (zombie.transform.position - tapPosition).magnitude < (wantedZombie.transform.position - tapPosition).magnitude)
                        wantedZombie = zombie;
                DeactivateAims();
                yield return currentPerson.Shoot(wantedZombie);
                IsStepOver = true;
            }
            havePlayerMadeDecision = false;
            yield return null;
        }
    }
    IEnumerator LookAtStepFlow()
    {
        while (currentPerson && !currentPerson.stepInfo.Over)
            yield return null;
        if (onStepOver != null)
            onStepOver();
    }
    void StopPersonActivityCoroutines()
    {
        StopAllCoroutines();
        StartCoroutine(ShootButtonActivity());
    }
    bool TryMove()
    {
        Fire = false;
        return !Fire;
    }
    bool TryFire()
    {
        Fire = true;
        return Fire;
    }
    
    #region API
    public void ManagePersonStep(PersonActionController pers)
    {
        currentPerson = pers;
        if (!currentPerson)
        {
            if (onStepOver != null)
                onStepOver();
            return;
        }

        CalculateCurrentPersonAims();
        currentPerson.stepInfo.CalculateMoves();

        if (currentPerson.stepInfo.Over)
        {
            if (onStepOver != null)
                onStepOver();
            return;
        }
        GameInfo.GetPersonObserver().LookAtPoint(currentPerson.transform.position);
        //SetGetInCarButton();
        OffPersonsHBars();
        currentPerson.ShowBar();

        StartCoroutine(LookAtStepFlow());
        IsStepOver = false;
        havePlayerMadeDecision = false;
        if (!currentPerson.stepInfo.CanMove)
            Fire = true;
        else
            Fire = false;
        //if (TryMove())
        //    return;
        //if (TryFire())
        //    return;
    }
    public void StopManagePersonStep()
    {
        //StopAllCoroutines();
        StopPersonActivityCoroutines();
        if(currentPerson)
            currentPerson.HideBar();
        currentPerson = null;
        //SetGetInCarButton();
        DeactivateAims();
        field.DeactivateActiveCells();
    }
    public void GetInTheCar()
    {
        car.PutIn(currentPerson);
    }
    #endregion
    void OffPersonsHBars()
    {
        foreach (PersonActionController person in FindObjectsOfType<PersonActionController>())
            person.HideBar();
    }
    void CalculateCurrentPersonAims()
    {
        currentPerson.stepInfo.CalculateAims(FindObjectsOfType<ZombieController>());
    }
    IEnumerator ShootButtonActivity()
    {
        while (true)
        {
            if (!currentPerson)
                fireButton.interactable = false;
            else if (!currentPerson.stepInfo.CanMove && currentPerson.stepInfo.CanFire)
                fireButton.interactable = false;
            else
                fireButton.interactable = currentPerson &&
                    !havePlayerMadeDecision &&
                    currentPerson.stepInfo.CanFire;

            yield return null;
        }
    }

    void OnCancelClick()
    {
        //if (onStepOver != null)
        //    onStepOver();
        //return;
        if (currentPerson.stepInfo.CanMove)
        {
            Fire = false;
            return;
        }
        if (PotentialActivePersonsLeft())
        {
            if (!currentPerson.stepInfo.CanFire)
                currentPerson.stepInfo.HasShot = true;
            if (onStepOver != null)
                onStepOver();
            return;
        }
        currentPerson.stepInfo.HasShot = true;
        if (onStepOver != null)
            onStepOver();
    }
    void SetActiveControlButtons(bool val)
    {
        foreach (Button button in new List<Button> { fireButton, nextCharBut, endStepBut, healBut })
            button.gameObject.SetActive(val);
        cancelBut.gameObject.SetActive(!val);
    }
    bool PotentialActivePersonsLeft()
    {
        foreach (PersonActionController person in FindObjectsOfType<PersonActionController>())
        {
            person.stepInfo.CalculateAims(FindObjectsOfType<ZombieController>());
            person.stepInfo.CalculateMoves();
            if (person.Alive && 
                person != currentPerson && 
                !(person.stepInfo.HasShot || 
                !person.stepInfo.CanMove && 
                !person.stepInfo.CanFire))
                return true;
        }
        return false;
    }
}