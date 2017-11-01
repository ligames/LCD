using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameGlobalInfo", menuName = "GlobalInfo")]
public class GlobalInfo : ScriptableObject
{
    [SerializeField]
    Transform carsContainer;
    [SerializeField]
    Transform charsContainer;
    [SerializeField]
    Transform zombiesContainer;
    [SerializeField]
    Transform gunsContainer;
    [SerializeField]
    Transform allResourcesContainer;
    [SerializeField]
    Transform tilesContainer;
    [SerializeField]
    int travellingSceneIndex = 1;
    [SerializeField]
    float velumProcessingTime = 1f;
    [SerializeField]
    TapType tapType;
    [SerializeField]
    float medicalRestoringValue = 1.5f;
    [SerializeField]
    string movingHint = "PRESS FIELD TO MOVE";
    [SerializeField]
    string shootingHint = "CHOOSE TARGET";
    [SerializeField]
    GameObject cellPrefab;

   
    public Transform CarsContainer { get { return carsContainer; } }
    public Transform CharsContainer { get { return charsContainer; } }
    public Transform ZombiesContainer { get { return zombiesContainer; } }
    public Transform GunsContainer { get { return gunsContainer; } }
    public Transform AllResourcesContainer { get { return allResourcesContainer; } }
    public Transform TilesContainer { get { return tilesContainer; } }

    public string MovingHint { get { return movingHint; } }
    public string ShootingHint { get { return shootingHint; } }

    public GameObject CellPrefab { get { return cellPrefab; } }

    public float VelumProcessingTime { get { return velumProcessingTime; } }
    public int TravellingSceneIndex { get { return travellingSceneIndex; } }
    public ITap TapInput
    {
        get
        {
            switch (tapType)
            {
                case (TapType.Mouse):
                        return new MouseTap();
                default:
                        return new TouchTap();
            }
        }
    }
    public float MedicalRestoringValue { get { return medicalRestoringValue; } }
}
