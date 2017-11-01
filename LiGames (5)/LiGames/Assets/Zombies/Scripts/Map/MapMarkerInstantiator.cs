using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMarkerInstantiator : MonoBehaviour
{
    [SerializeField]
    GameObject markerButtonPrefab;
    [SerializeField]
    Transform markersContainer;
    List<LocationMarkerUIControl> createdMarkers;
    Button button;

    void Awake()
    {
        createdMarkers = new List<LocationMarkerUIControl>();
    }
    public bool IsAtLeasOneLocationAvailable
    {
        get
        {
            if (createdMarkers == null || createdMarkers.Count == 0)
                return true;
            foreach (LocationMarkerUIControl marker in createdMarkers)
                if (marker.Available)
                    return true;
            return false;
        }
    }

    void InstantiateLocationMarker(Location startLoc, Location destination)
    {
        GameObject marker = InstantiateMarker
        (
            WorldToScreenVec3(destination.SkinMount.position)
        );
        LocationMarkerUIControl uiMarkerController = marker.GetComponent<LocationMarkerUIControl>();
        uiMarkerController.StartLocation = startLoc;
        uiMarkerController.Location = destination;
        
        createdMarkers.Add(uiMarkerController);
    }
    public void InstantiateLocationNeighboursMarkers(Location location)
    {
        //createdMarkers.Clear();
        foreach (Path path in location.GetNeighbours())
            InstantiateLocationMarker(location, path.Destination);
    }
    public void HideAllMarkers()
    {
        foreach (LocationMarkerUIControl marker in createdMarkers)
            marker.gameObject.SetActive(false);
            //Destroy(marker.gameObject);
        createdMarkers.Clear();
    }
    GameObject InstantiateMarker(Vector3 where)
    {
        GameObject marker = Instantiate(markerButtonPrefab, markersContainer);
        marker.transform.position = where;
        return marker;
    }

    Vector3 WorldToScreenVec3(Vector3 worldVec3)
    {
        Vector3 screenVec3 = Camera.main.WorldToScreenPoint(worldVec3);
        return screenVec3;
    }
}
