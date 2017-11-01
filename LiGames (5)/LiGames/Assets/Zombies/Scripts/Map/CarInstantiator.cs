using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarInstantiator : MonoBehaviour
{
    [SerializeField]
    GameObject carNavAgentPrefab;
    [SerializeField]
    Transform carsPrefabsContainer;
    List<GameObject> carsPrefabs;

    void Awake()
    {
        InitializeCarsPrefabs();
    }

    public GameObject InstantiateCar(int carId, Transform mount, out NavMeshAgent agent)
    {
        GameObject carNavAgent = Instantiate(carNavAgentPrefab);
        carNavAgent.transform.position = mount.position;
        carNavAgent.transform.rotation = mount.rotation;
        agent = carNavAgent.GetComponent<NavMeshAgent>();

        return Instantiate(carsPrefabs[carId], carNavAgent.transform);
    }

    void InitializeCarsPrefabs()
    {
        carsPrefabs = new List<GameObject>();
        for (int i = 0; i < carsPrefabsContainer.childCount; ++i)
            carsPrefabs.Add(carsPrefabsContainer.GetChild(i).gameObject);
    }
}
