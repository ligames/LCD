using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GlobalInfoAccessor : MonoBehaviour
{
    [Inject]
    GlobalInfo globalInfo;

    List<GameObject> carsPrefabs;
    public List<GameObject> CarsPrefabs
    {
        get
        {
            LazyInit();
            return carsPrefabs;
        }
    }
    List<GameObject> tilesPrefabs;
    public List<GameObject> TilesPrefabs
    {
        get
        {
            LazyInit();
            return tilesPrefabs;
        }
    }
    List<PersonDescriptor> charsPrefabs;
    public List<PersonDescriptor> CharsPrefabs
    {
        get
        {
            LazyInit();
            return charsPrefabs;
        }
    }
    List<GameObject> zombiesPrefabs;
    public List<GameObject> ZombiesPrefabs
    {
        get
        {
            LazyInit();
            return zombiesPrefabs;
        }
    }
    List<GunResourceInfo> gunsResources;
    public List<GunResourceInfo> GunsResources
    {
        get
        {
            LazyInit();
            return gunsResources;
        }
    }
    List<ResourceInfo> allResources;
    public List<ResourceInfo> AllResources
    {
        get
        {
            LazyInit();
            return allResources;
        }
    }

    bool initialized = false;
    void LazyInit()
    {
        if (!initialized)
        {
            initialized = true;
            Init();
        }
    }
    void Init()
    {
        carsPrefabs = CreatePrefabList(globalInfo.CarsContainer);
        charsPrefabs = CreatePersonDescriptorsList(globalInfo.CharsContainer);
        zombiesPrefabs = CreatePrefabList(globalInfo.ZombiesContainer);
        gunsResources = new List<GunResourceInfo>();
        for (int i = 0; i < globalInfo.GunsContainer.childCount; ++i)
            gunsResources.Add(globalInfo.GunsContainer.GetChild(i).GetComponent<GunResourceInfo>());
        allResources = new List<ResourceInfo>();
        for (int i = 0; i < globalInfo.AllResourcesContainer.childCount; ++i)
            allResources.Add(globalInfo.AllResourcesContainer.GetChild(i).GetComponent<ResourceInfo>());
        foreach (GunResourceInfo gun in GunsResources)
            allResources.Add(gun);
        tilesPrefabs = new List<GameObject>();
        for (int i = 0; i < globalInfo.TilesContainer.childCount; ++i)
            tilesPrefabs.Add(globalInfo.TilesContainer.GetChild(i).gameObject);
    }

    List<GameObject> CreatePrefabList(Transform container)
    {
        List<GameObject> prefabList = new List<GameObject>();
        for (int i = 0; i < container.childCount; ++i)
            prefabList.Add(container.GetChild(i).gameObject);
        return prefabList;
    }
    List<PersonDescriptor> CreatePersonDescriptorsList(Transform container)
    {
        List<PersonDescriptor> prefabList = new List<PersonDescriptor>();
        for (int i = 0; i < container.childCount; ++i)
            prefabList.Add(container.GetChild(i).GetComponent<PersonDescriptor>());
        return prefabList;
    }
    public int GetGunId(GunResourceInfo info)
    {
        for (int i = 0; i < GunsResources.Count; ++i)
            if (GunsResources[i].Equals(info))
                return i;
        return -1;
    }
    public int GetTileId(GameObject tile)
    {
        for (int i = 0; i < TilesPrefabs.Count; ++i)
            if (TilesPrefabs[i] == tile)
                return i;
        Debug.Log("tile not found");
        return -1;
    }
    public float MedicalRestoringValue { get { return globalInfo.MedicalRestoringValue; } }
}
