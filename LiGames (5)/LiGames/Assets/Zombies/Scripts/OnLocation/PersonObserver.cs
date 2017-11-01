using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PersonObserver : MonoBehaviour
{
    [Inject]
    CameraMovementConfig config;

    Vector3 offset;
    GameInfo gameInfo;
    Vector3 destination;

    void Awake()
    {
        gameInfo = FindObjectOfType<GameInfo>();
        InitializeCenterOfView();
        StartCoroutine(FlowCameraToDestination());
    }
    void InitializeCenterOfView()
    {
        RaycastHit[] hits;
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits)
            if(hit.collider.tag == GameInfo.terrainTag)
            {
                Vector3 centerOfView = hit.point;
                offset = Camera.main.transform.position - centerOfView;
                return;
            }
        Debug.LogError("Terrain object is missing on the scene.");
    }
    IEnumerator FlowCameraToDestination()
    {
        Vector3 velocity = Vector3.zero;
        while (true)
        {
            MoveCamerasToThePoint(ref velocity);
            yield return null;
        }
    }
    bool AreCamerasInThePoint()
    {
        foreach (Camera camera in Camera.allCameras)
            if (camera.transform.position != destination)
                return false;
        return true;
    }
    void MoveCamerasToThePoint(ref Vector3 vel)
    {
        Vector3 newPos = Vector3.SmoothDamp(
                Camera.main.transform.position,
                destination,
                ref vel,
                config.DeltaTime);
        foreach (Camera camera in Camera.allCameras)
            camera.transform.position = newPos;
    }   
    public void LookAtPoint(Vector3 point)
    {
        destination = point + offset;
    }
}
