using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LocationManager : MonoBehaviour
{
    [Inject]
    GlobalInfo globalInfo;
    [Inject]
    Velum velum;
    [SerializeField]
    Button healButton;
    [SerializeField]
    Text healPosibilityText;
    [Inject]
    PersonStepManager stepManager;
    [Inject]
    BackpackContents backpack;
    [Inject]
    PersonActionManager actionManager;

    public Instantiator instantiator;
    public Transform carMount;
    public GameObject personsPositionsContainer;
    public GameObject carTapColliderHandler;
    public State state;

    List<Transform> personsPositions;
    GameObject carPhysics;

    void Awake()
    {
        personsPositions = new List<Transform>();
        for (int i = 0; i < personsPositionsContainer.transform.childCount; ++i)
            personsPositions.Add(personsPositionsContainer.transform.GetChild(i));

        for (int i = 0; i < personsPositions.Count && i < state.Chars.Count; ++i)
            instantiator.InstantiateChar(personsPositions[i], state.Chars[i]);
        healButton.onClick.AddListener(new UnityEngine.Events.UnityAction(HealPerson));
        StartCoroutine(HealingPosibilityObserving());
    }

    void Start()
    {
        carPhysics = instantiator.InstantiateCar(carMount, state.CarState.Id);
        ResizeCarCollider();
    }
    void ResizeCarCollider()
    {
        BoxCollider collider = carTapColliderHandler.GetComponent<BoxCollider>();
        collider.size = carPhysics.GetComponent<MeshRenderer>().bounds.size;
        collider.transform.position = carPhysics.transform.position;
    }
    public void LoadTravellingScene()
    {
        StartCoroutine(TravelSceneLoading());
    }
    IEnumerator TravelSceneLoading()
    {
        yield return velum.Close();
        SceneManager.LoadScene(globalInfo.TravellingSceneIndex);
    }
    IEnumerator HealingPosibilityObserving()
    {
        while(true)
        {
            //healAllowed = !stepManager.IsStepOver;
            //healAllowed = healAllowed && backpack.MedicalCount > 0;
            bool healAllowed = actionManager.CurrentPerson && 
                !stepManager.havePlayerMadeDecision &&
                backpack.MedicalCount > 0;

            healButton.interactable = healAllowed;
            bool textVisible = backpack.MedicalCount >= 1;
            if (textVisible)
                healPosibilityText.text = backpack.MedicalCount.ToString();
            healPosibilityText.gameObject.SetActive(textVisible);
            yield return null;
        }
    }
    void HealPerson()
    {
        StartCoroutine(actionManager.CurrentPerson.UseMedical());
    }
}