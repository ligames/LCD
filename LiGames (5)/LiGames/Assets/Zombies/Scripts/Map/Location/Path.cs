using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField]
    Location destination;
    [SerializeField]
    float distance;

    public Path(Location loc, float dis)
    {
        destination = loc;
        distance = dis;
    }

    public Location Destination { get { return destination; } }
    public float Distance { get { return distance; } }
}
