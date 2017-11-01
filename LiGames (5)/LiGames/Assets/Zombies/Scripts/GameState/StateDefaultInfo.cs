using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StateDefaults", menuName = "State defaults config")]
public class StateDefaultInfo : ScriptableObject
{
    [SerializeField]
    int personsCount = 2;
    [SerializeField]
    int minFood = 40;
    [SerializeField]
    int maxFood = 110;
    [SerializeField]
    int minFuel = 100;
    [SerializeField]
    int maxFuel = 155;
    [SerializeField]
    float minCarSpeed = 10;
    [SerializeField]
    float maxCarSpeed = 15;

    public int PersonsCount { get { return personsCount; } }
    public int MinFood { get { return minFood; } }
    public int MaxFood { get { return maxFood; } }
    public int MinFuel { get { return minFuel; } }
    public int MaxFuel { get { return maxFuel; } }
    public float MinCarSpeed { get { return minCarSpeed; } }
    public float MaxCarSpeed { get { return maxCarSpeed; } }
}
