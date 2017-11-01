using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView
{
    public static bool IsVisiblePoint(Vector3 point)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);
        return screenPoint.x >= 0.0f && screenPoint.y >= 0.0f &&
            screenPoint.x <= Screen.width && screenPoint.y <= Screen.height;
    }
}

public class Spawner : MonoBehaviour
{
    Instantiator instantiator;

    public bool IsVisible
    {
        get
        {
            return CameraView.IsVisiblePoint(transform.position);
        }
    }

    void Awake()
    {
        instantiator = FindObjectOfType<Instantiator>();
    }
    public void SpawnZombie()
    {
        instantiator.InstantiateZombie(transform);
    }
}
