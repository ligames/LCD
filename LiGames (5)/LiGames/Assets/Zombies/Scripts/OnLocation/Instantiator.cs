using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Instantiator : MonoBehaviour
{
    [Inject]
    PersonActionManager manager;
    [Inject]
    GlobalInfoAccessor globalInfo;

    public GameObject charPrefab, zombiePrefab;

    List<GameObject> InitializeFromContainer(GameObject container)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < container.transform.childCount; ++i)
            list.Add(container.transform.GetChild(i).gameObject);
        return list;
    }
    public GameObject InstantiateCar(Transform mount, int id)
    {
        return Instantiate
        (
            globalInfo.CarsPrefabs[id],
            mount
        );
    }
    public void InstantiateChar(Transform mount, PersonState state)
    {
        GameObject character = Instantiate(charPrefab, mount.position, mount.rotation);
        GameObject charGO = Instantiate
        (
            globalInfo.CharsPrefabs[state.Id].Prefab,
            character.transform
        );
        PersonActionController charPersonComp = character.GetComponentInChildren<PersonActionController>();
        manager.RegisterPerson(charPersonComp, state.Id);
        charPersonComp.State = state;
    }
    public void InstantiateZombie(Transform mount)
    {
        GameObject zombieP = Instantiate(zombiePrefab, mount.position, mount.rotation);
        System.Random numGen = new System.Random();
        int zombieId = numGen.Next(globalInfo.ZombiesPrefabs.Count);
        Instantiate(globalInfo.ZombiesPrefabs[zombieId], zombieP.transform);
    }
}
