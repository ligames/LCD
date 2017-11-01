using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Car : MonoBehaviour
{
    [Inject]
    LocationManager locationManager;
    [Inject]
    GlobalInfo globalInfo;

    public PersonActionManager manager;
    [Inject]
    PersonStepManager stepManager;
    public int carPhysicLayerNum = 9;
    public GameObject carPhysic;
    public DialogWindow carDialog;
    public Illumination illumination;
    
    public bool IsTapped { get; private set; }

    GameInfo gameInfo;

    bool ArePeopleInArea
    {
        get
        {
            foreach (PersonActionController person in FindObjectsOfType<PersonActionController>())
                if (person && person.Alive && person.IsInCarArea)
                    return true;
            return false;
        }
    }

    int peopleCount = 0;

    //ITap tapInput;

    void Awake()
    {
        gameInfo = FindObjectOfType<GameInfo>();
        //tapInput = new MouseTap();
        carDialog.HeaderString = "ATTENTION";
        carDialog.YesString = "YES,GET OUT";
        carDialog.NoString = "NO,I'M STAY";
        carDialog.MessageString = "DO YOU REALLY WANT TO LEAVE YOUR PERSON HERE?\nYOU'LL LOSE HERO FOREVER";
        carDialog.onClick += OnCarDialogResponse;
    }
    void Start()
    {
        illumination.On();
    }

    void OnTriggerEnter(Collider c)
    {
        PersonActionController person = c.GetComponent<PersonActionController>();
        if (person && person.Alive)
            person.IsInCarArea = true;
        if (ArePeopleInArea && !illumination.IsOn)
            illumination.On();
    }
    void OnTriggerExit(Collider c)
    {
        PersonActionController person = c.GetComponent<PersonActionController>();
        if (person && person.Alive)
            person.IsInCarArea = false;
        if (!ArePeopleInArea && illumination.IsOn)
            illumination.Off();
    }
    void Update()
    {
        IsTapped = false;
        TapPosition tapPos = globalInfo.TapInput.GetTap();
        if (tapPos != null)
        {
            if (GameInfo.IsTapOnUI(tapPos.Position)) return;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(tapPos.Position);
            if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << carPhysicLayerNum))
                if (hit.collider.gameObject == carPhysic)
                    OnTap();
        }
    }
    void OnTap()
    {
        IsTapped = true;
        if (stepManager.havePlayerMadeDecision)
            return;
        if (manager.CurrentPerson)
            if (manager.CurrentPerson.IsInCarArea)
                PutIn(manager.CurrentPerson);
            else if (peopleCount > 0)
                carDialog.Show();
    }
    void OnCarDialogResponse()
    {
        if (carDialog.Result == DialogResult.Yes)
        {
            foreach (PersonActionController person in FindObjectsOfType<PersonActionController>())
                if (person.Alive)
                    manager.CharDied(person);
            DriveAway();
        }
    }
    void DriveAway()
    {
        locationManager.LoadTravellingScene();
    }
    #region API
    public void PutIn(PersonActionController person)
    {
        person.IsInCarArea = false;
        manager.CharRemoved(person);
        ++peopleCount;
        Destroy(person.gameObject);
        if (!manager.IsAnyPersonOnField)
            DriveAway();
        if (!ArePeopleInArea && illumination.IsOn)
            illumination.Off();
    }
    public void CharRemovedFromLocation()
    {
        if (!manager.IsAnyPersonOnField && peopleCount > 0)
            DriveAway();
    }
    #endregion
}
