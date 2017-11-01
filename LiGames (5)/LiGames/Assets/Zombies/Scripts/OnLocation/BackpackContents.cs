using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BackpackContents : SaveableBehavior
{
    [Inject]
    GlobalInfoAccessor globalInfo;
    int expendableItemsCount = 0;
    List<ResourceInfo> resources;
    public List<ResourceInfo> Resources
    {
        get
        {
            if (resources == null)
                resources = new List<ResourceInfo>();
            return resources;
        }
        private set
        {
            resources = value;
        }
    }
    public int MedicalCount { get; private set; }
    public bool ContainExpendable
    {
        get
        {
            return expendableItemsCount > 0;
        }
    }
    new void Awake()
    {
        base.Awake();
    } 
    public void AddItem(ResourceInfo res, PersonActionController who)
    {
        if (res.GetType() == typeof(GunResourceInfo))
        {
            GunResourceInfo gun = (GunResourceInfo)res;
            who.Adopt(gun);
        }
        else
            AddItem(res);
    }
    public void AddItem(ResourceInfo res)
    {
        if (!res)
            return;
        if (res.GetType() == typeof(MedicalInfo))
        {
            ++MedicalCount;
            return;
        }
        Resources.Add(res);
        if (res.expendable)
            ++expendableItemsCount;
    }
    public void RemoveItem(ResourceInfo res)
    {
        Resources.Remove(res);
        //if (res.expendable)
        //    --expendableItemsCount;
    }

    public override void Save()
    {
        if (PlayerPrefs.GetInt(SavingHelper.Backpack) > 0 && !loaded)
            return;
        PlayerPrefs.SetInt(SavingHelper.Backpack, 1);
        PlayerPrefs.SetInt(SavingHelper.ResourcesCount, Resources.Count);
        for (int i = 0; i < Resources.Count; ++i)
            PlayerPrefs.SetInt(SavingHelper.Resource + i.ToString(),
                GetPrefabIndex(Resources[i]));
        PlayerPrefs.SetInt(SavingHelper.Resource + SavingHelper.Medical, MedicalCount);
    }
    bool loaded = false;
    public override void Load()
    {
        loaded = true;
        int count = PlayerPrefs.GetInt(SavingHelper.ResourcesCount);
        for(int i = 0; i < count; ++i)
        {
            int index = PlayerPrefs.GetInt(SavingHelper.Resource + i.ToString());
            Resources.Add(globalInfo.AllResources[index]);
        }
        MedicalCount = PlayerPrefs.GetInt(SavingHelper.Resource + SavingHelper.Medical);
    }
    public void UseMedical()
    {
        --MedicalCount;
    }
    int GetPrefabIndex(ResourceInfo info)
    {
        for (int i = 0; i < globalInfo.AllResources.Count; ++i)
            if (info.IsEquals(globalInfo.AllResources[i]))
                return i;
        return -1;
    }
}
