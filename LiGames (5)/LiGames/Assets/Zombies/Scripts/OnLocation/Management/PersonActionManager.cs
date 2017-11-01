using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

#region Tap identification
public class TapPosition
{
    public Vector3 Position { get; set; }
}

public interface ITap
{
    TapPosition CurrentPosition { get; }
    TapPosition GetTap();
    TapPosition GetTapUp();
}

class MouseTap : ITap
{
    public TapPosition CurrentPosition
    {
        get
        {
            return new TapPosition { Position = Input.mousePosition };
        }
    }
    public TapPosition GetTap()
    {
        if (Input.GetMouseButtonDown(0))
            return new TapPosition { Position = Input.mousePosition };
        return null;
    }
    public TapPosition GetTapUp()
    {
        if (Input.GetMouseButtonUp(0))
            return new TapPosition { Position = Input.mousePosition };
        return null;
    }
}

class TouchTap : ITap
{
    public TapPosition CurrentPosition
    {
        get
        {
            TapPosition res = null;
            if (Input.touchCount == 1)
                res = new TapPosition { Position = Input.GetTouch(0).position };
            return res;
        }
    }
    public TapPosition GetTap()
    {
        if (Input.touchCount != 1)
            return null;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            Debug.Log("tap");
            return new TapPosition { Position = touch.position };
        }
        return null;
    }
    public TapPosition GetTapUp()
    {
        if (Input.touchCount != 1)
            return null;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Ended)
            return new TapPosition { Position = touch.position };
        return null;
    }
}
#endregion

public class PersonSwitcher
{
    delegate int Calculator(int num);
    List<PersonActionController> persons;
    List<PersonActionController> personsToDelete;
    int carriage = 0;
    bool end = false;
    PersonStepManager stepManager;
    public PersonActionManager actionManager { get; set; }
    public PersonSwitcher(PersonStepManager stepManager)
    {
        persons = new List<PersonActionController>();
        personsToDelete = new List<PersonActionController>();
        this.stepManager = stepManager;
        this.stepManager.onStepOver += Next;
    }
    public PersonActionController Current
    {
        get
        {
            if (!IsStepOver && carriage < persons.Count)
                return persons[carriage];
            return null;
        }
    }
    public bool IsAnyAlive
    {
        get
        {
            foreach (PersonActionController p in persons)
                if (p && p.Alive && !personsToDelete.Contains(p))
                    return true;
            return false;
        }
    }
    bool IsStepOver
    {
        get
        {
            if (!end)
                foreach (PersonActionController person in persons)
                    if (person && !person.stepInfo.Over)
                        return false;
            RemovePersons();
            return true;
        }
    }
    void RemovePersons()
    {
        foreach (PersonActionController p in personsToDelete)
            persons.Remove(p);
        personsToDelete.Clear();
        carriage = 0;
    }
    public void AddPerson(PersonActionController p)
    {
        persons.Add(p);
    }
    public void DeletePerson(PersonActionController p)
    {
        personsToDelete.Add(p);
    }
    public IEnumerator ManagePersonsSteps()
    {
        end = false;
        RemovePersons();
        foreach (PersonActionController person in persons)
            person.stepInfo.StartStep();
        stepManager.ManagePersonStep(Current);
        while (!IsStepOver)
            yield return null;
        stepManager.StopManagePersonStep();
    }
    public void Next()
    {
        if (Current && !Current.stepInfo.CanMove && !Current.stepInfo.CanFire)
            Current.stepInfo.HasShot = true;
        Switch((nextNum) => (nextNum + 1) % persons.Count);
    }
    public void Back()
    {
        Switch((prevNum) => prevNum - 1 < 0 ? persons.Count - 1 : prevNum - 1);
    }
    public void End()
    {
        end = true;
    }
    void Switch(Calculator getNextIndex)
    {
        if (IsStepOver || stepManager.havePlayerMadeDecision)
            return;

        int nextNum = carriage;
        PersonActionController nextPerson = null;
        do
        {
            nextNum = getNextIndex(nextNum);
            nextPerson = persons[nextNum];
            if (nextPerson == Current)
                return;
        }
        while (!nextPerson || nextPerson.stepInfo.Over);
        stepManager.StopManagePersonStep();
        carriage = nextNum;
        stepManager.ManagePersonStep(Current);
    }
}

public class PersonActionManager : MonoBehaviour
{
    LocationManager manager;
    LocationManager Manager
    {
        get
        {
            if (!manager)
                manager = FindObjectOfType<LocationManager>();
            return manager;
        }
        set
        {
            manager = value;
        }
    }
    [Inject]
    Field field;
    public Field Field { get { return field; } }
    [Inject]
    GameInfo gameInfo;
    public GameInfo GameInfo { get { return gameInfo; } }
    public BackpackContents BackPack { get; private set; }
    [Inject]
    PersonStepManager stepManager;
    public Button nextPersonButton, endStepButton;
    public GameObject[] cameras;
    public Car car;
    GameObject currentShootingCamera;
    public GameObject shootingCameraPrefab;
    PersonSwitcher charsSwitcher;
    [SerializeField]
    PersonObserver observer;
    Generator generator;
    [Inject]
    LoadingAnimator loadingAnimator;
    Dictionary<Person, int> personsRegistry;
    [Inject]
    State state;
    [SerializeField]
    DialogWindow gameOverDialogWindow;

    public PersonActionController CurrentPerson
    {
        get
        {
            return charsSwitcher == null ?
                null :
                charsSwitcher.Current;
        }
    }
    public bool IsAnyPersonOnField { get { return charsSwitcher.IsAnyAlive; } }

    void Awake()
    {
        BackPack = GetComponent<BackpackContents>();
        generator = FindObjectOfType<Generator>();
        gameOverDialogWindow.onClick += Manager.LoadTravellingScene;
    }
    void Start()
    {
        InitiatePersonsGroups();
        nextPersonButton.onClick.AddListener(new UnityEngine.Events.UnityAction(charsSwitcher.Next));
        endStepButton.onClick.AddListener(new UnityEngine.Events.UnityAction(charsSwitcher.End));

        StartCoroutine(StepCycle());
        StartCoroutine(ButtonInteractMonitoring());
    }
    IEnumerator StepCycle()
    {
        while (true)
        {
            yield return charsSwitcher.ManagePersonsSteps();

            generator.ExecuteGen();

            List<ZombieController> zombies = new List<ZombieController>();
            foreach (ZombieController zombie in FindObjectsOfType<ZombieController>())
                if(zombie.Alive)
                    zombies.Add(zombie);

            foreach (ZombieController zombie in zombies)
                zombie.MakeDecision();
            loadingAnimator.ShowLoading();
            foreach (ZombieController zombie in zombies)
                zombie.DoDecision();
            while (!WereZombiesFinishedSteps(zombies))
                yield return null;
            field.FreeStakedOutCells();
            loadingAnimator.StopLoading();
        }
    }

    bool WereZombiesFinishedSteps(List<ZombieController> zombies)
    {
        foreach (ZombieController zombie in zombies)
            if (!zombie.StepIsOver)
                return false;
        return true;
    }

    void InitiatePersonsGroups()
    {
        charsSwitcher = new PersonSwitcher(stepManager) { actionManager = GetComponent<PersonActionManager>() };
        foreach (PersonActionController person in FindObjectsOfType<PersonActionController>())
            charsSwitcher.AddPerson(person);
    }

    IEnumerator ButtonInteractMonitoring()
    {
        SetActiveSwitchButtons(false);
        bool active = false;
        bool newActive = false;
        while(true)
        {
            if (CurrentPerson)
                if (stepManager.havePlayerMadeDecision)
                    newActive = false;
                else
                    newActive = true;
            else
                newActive = false;

            if(newActive != active)
            {
                active = newActive;
                SetActiveSwitchButtons(active);
            }
            
            yield return null;
        }
    }

    #region API
    public void BindPersonToField(Person p)
    {
        field.CorrectPersonPosition(p);
    }
    public void PersonPositionChanged(Person p)
    {
        field.RegisterPersonPosition(p, field.NearestCell(p.transform.position));
    }
    public void CharRemoved(PersonActionController p)
    {
        charsSwitcher.DeletePerson(p);
        RemovePersonFromField(p);
        car.CharRemovedFromLocation();
    }
    public void CharDied(PersonActionController p)
    {
        state.RemoveChar(personsRegistry[p]);
        personsRegistry.Remove(p);

        /////////////////////////
        //Game Over
        if (personsRegistry.Count == 0)
        {
            SavingHelper.GameOver();
            gameOverDialogWindow.YesString = "Ok";
            gameOverDialogWindow.HeaderString = "GAME OVER";
            gameOverDialogWindow.MessageString = "ALL OF YOUR'S PERSONS ARE DEAD";
            gameOverDialogWindow.Show();
        }
        /////////////////////////
    }
    public void RemovePersonFromField(Person p)
    {
        field.RemovePerson(p);
    }
    public void SetActiveSwitchButtons(bool val)
    {
        nextPersonButton.interactable =
            endStepButton.interactable = val;
    }
    //public void ShootingFilming(Transform mount)
    //{
    //    foreach (GameObject camera in cameras)
    //        camera.SetActive(false);
    //    currentShootingCamera = Instantiate(shootingCameraPrefab, mount);
    //}
    public void StopShotFilming()
    {
        Destroy(currentShootingCamera);
        foreach (GameObject camera in cameras)
            camera.SetActive(true);
    }
    public void RegisterPerson(Person p, int id)
    {
        if (personsRegistry == null)
            personsRegistry = new Dictionary<Person, int>();
        personsRegistry.Add(p, id);
    }
    #endregion
}
