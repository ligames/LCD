using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Zenject;

public class TravellingManager : MonoBehaviour
{
    [Inject]
    MapGenerator generator;
    [Inject]
    State state;
    [Inject]
    MapMarkerInstantiator markerInsantiator;
    [Inject]
    GameInfo gameInfo;
    [Inject]
    NavMeshBaker baker;
    [Inject]
    CarInstantiator carInstantiator;
    [Inject]
    IPerformanceImprovable performanceImprover;
    [Inject]
    Velum velum;
    [Inject]
    GlobalInfo globalInfo;
    [Inject]
    DialogWindow gameOverDialogWindow;

    Transform carTransform;
    NavMeshAgent carNavAgent;
    LocationMarkerUIControl activeLocationPoint;
    void Awake()
    {
        state.onSpeedChanged += CalculateEveryLocation;
        gameOverDialogWindow.onClick += () =>
            StartCoroutine(LoadTravelScene());
    }
    void Start()
    {
        generator.CreateWorld();
        Location currentLocation = GetCurrentLocation();

        if (IsBorderLocation(currentLocation))
            generator.BindCenterToEdges();
        //baker.BakeAllSurfaces();
        carTransform = carInstantiator.InstantiateCar
            (
                state.CarState.Id, 
                currentLocation.CarMount, 
                out carNavAgent
            ).transform;
        carNavAgent.Warp(currentLocation.CarMount.position);
        SetCameraPositionAtCar();
        markerInsantiator.InstantiateLocationNeighboursMarkers(currentLocation);
        StartCoroutine(ObserveGamerPossibilityToPlay());
        //performanceImprover.ImprovePerformance();
    }
    void SetCameraPositionAtCar()
    {
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        hits = Physics.RaycastAll(ray);
        Vector3 terrainPoint = Vector3.zero;
        foreach (RaycastHit hit in hits)
            if (hit.collider.tag == GameInfo.terrainTag)
            {
                Debug.Log("found");
                terrainPoint = hit.point;
                break;
            }
        Vector3 offset = Camera.main.transform.position - terrainPoint;
        Camera.main.transform.position = carTransform.position + offset;

        //Vector3 carInCameraPos = Camera.main.WorldToScreenPoint(carTransform.position);
        //Vector3 centerOfView = Camera.main.WorldToScreenPoint(Camera.main.transform.position);
    }
    Location GetCurrentLocation()
    {
        if (!SavingHelper.IsGameSaved)
            return generator.GenerateRandomLocation();

        int tileX = PlayerPrefs.GetInt(SavingHelper.TileCenter + SavingHelper.XCoordinate);
        int tileY = PlayerPrefs.GetInt(SavingHelper.TileCenter + SavingHelper.YCoordinate);
        int locationIndex = PlayerPrefs.GetInt(
            SavingHelper.XCoordinate + tileX + SavingHelper.YCoordinate + tileY +
            SavingHelper.Tile + SavingHelper.Location);

        return generator.Center[locationIndex];
    }
    bool IsBorderLocation(Location location)
    {
        return location.GetType() == typeof(BorderLocation);
    }
    IEnumerator ObserveGamerPossibilityToPlay()
    {
        while (true)
        {
            if(!markerInsantiator.IsAtLeasOneLocationAvailable)
            {
                SavingHelper.GameOver();
                gameOverDialogWindow.HeaderString = "GAME OVER";
                gameOverDialogWindow.MessageString = "YOU'RE WITHOUT NECESSARY AMOUNT OF FOOD OR/AND FUEL";
                gameOverDialogWindow.YesString = "Ok";
                gameOverDialogWindow.Show();
                yield break;
            }
            yield return null;
        }
    }
    IEnumerator DrivingCar()
    {
        Time.timeScale = 3;
        carNavAgent.SetDestination(activeLocationPoint.Location.CarMount.position);
        if(!(activeLocationPoint.Location.GetType() == activeLocationPoint.StartLocation.GetType() && 
            activeLocationPoint.Location.GetType() == typeof(BorderLocation)))
            while ((carNavAgent.gameObject.transform.position -
                activeLocationPoint.Location.CarMount.position).magnitude > GetError())
                yield return null;
        yield return velum.Close();
        Time.timeScale = 1;
        activeLocationPoint.Location.Descriptor.EnterIntoLocation();
    }
    public void DriveToNextLocation()
    {
        Debug.Log("drive to next location");
        markerInsantiator.HideAllMarkers();
        state.UseFood(activeLocationPoint.FoodCost);
        state.UseFuel(activeLocationPoint.FuelCost);
        generator.UpdateCenter(activeLocationPoint.Location);
        StartCoroutine(DrivingCar());
    }
    public void OnLocationPointClick(LocationMarkerUIControl lPoint)
    {
        Debug.Log("On loc point click");
        if (activeLocationPoint)
            activeLocationPoint.Deactivate();
        activeLocationPoint = lPoint;
    }
    void CalculateEveryLocation(float speed)
    {
        foreach (LocationMarkerUIControl loc in FindObjectsOfType<LocationMarkerUIControl>())
            loc.CalculateResources(speed);
    }
    float GetError()
    {
        return 15f;
    }
    IEnumerator LoadTravelScene()
    {
        yield return velum.Close();
        SceneManager.LoadScene(globalInfo.TravellingSceneIndex);
    }
}
