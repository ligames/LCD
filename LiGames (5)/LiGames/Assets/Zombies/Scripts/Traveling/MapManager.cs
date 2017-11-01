using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    public float carDestinationError = 0.5f;
    public State state;
    public State State { get { return state; } }
    LocationPoint activeLocationPoint;
    public NavMeshAgent carNavMeshAgent;
    public Instantiator instantiator;
    public Transform carMount;
    public GameObject locationsContainer;
    List<LocationPoint> locationsPoints;

    void Awake()
    {
        state.onSpeedChanged += CalculateEveryLocation;
        InitializeLocations();
    }
    void Start()
    {
        //LocationPoint locPoint = locationsPoints[state.locationId];
        instantiator.InstantiateCar(carMount, state.CarState.Id);
        //Vector3 newPos = locPoint.realLocTransform.position;
        //newPos.y = carMount.position.y;
        //carMount.position = newPos;
    }

    public void OnLocationPointClick(LocationPoint lPoint)
    {
        if (activeLocationPoint)
            activeLocationPoint.Deactivate();
        activeLocationPoint = lPoint;
    }
    public void DriveToNextLocation()
    {
        foreach(LocationPoint point in FindObjectsOfType<LocationPoint>())
        {
            point.Deactivate();
            point.Interactable = false;
        }
        StartCoroutine(DrivingCar());
    }
    IEnumerator DrivingCar()
    {
        carNavMeshAgent.SetDestination(activeLocationPoint.realLocTransform.position);
        
        while ((carNavMeshAgent.gameObject.transform.position - 
            activeLocationPoint.realLocTransform.position).magnitude > carDestinationError)
            yield return null;
        yield return new WaitForSeconds(1.0f);
        //SceneManager.LoadScene(activeLocationPoint.sceneIndex);
    }
    void CalculateEveryLocation(float speed)
    {
        foreach (LocationPoint loc in FindObjectsOfType<LocationPoint>())
            loc.CalculateResources(speed);
    }
    void InitializeLocations()
    {
        locationsPoints = new List<LocationPoint>();
        for (int i = 0; i < locationsContainer.transform.childCount; ++i)
            locationsPoints.Add(locationsContainer.transform.GetChild(i).GetComponent<LocationPoint>());
    }
}
