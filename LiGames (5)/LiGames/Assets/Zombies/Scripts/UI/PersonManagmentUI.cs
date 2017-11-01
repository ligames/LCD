using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PersonManagmentUI : MonoBehaviour
{
    [Inject]
    GlobalInfoAccessor globalInfo;
    [Inject]
    InventoryUIController inventoryController;

    [SerializeField]
    RawImage icon;

    [SerializeField]
    GunSlotUI gunSlot;

    PersonState state;
    PersonDescriptor descriptor;

    void Start()
    {
        gunSlot.onAssign += OnGunSlotAssign;
    }

    public PersonState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            if (state == null)
                descriptor = null;
            else
                descriptor = globalInfo.CharsPrefabs[state.Id];
        }
    }

    public void Assign(PersonState newState)
    {
        State = newState;
        UpdateView();
    }
    void UpdateView()
    {
        UpdateIcon();
        UpdateGun();
    }
    void UpdateIcon()
    {
        if(descriptor)
            icon.texture = descriptor.Icon.texture;
        icon.gameObject.SetActive(descriptor);
    }
    void UpdateGun()
    {
        bool visible = state != null && state.GunId >= 0;
        if (visible)
            gunSlot.Assign
            (
                globalInfo.GunsResources[state.GunId]
            );
        else
            gunSlot.Assign(null);
    }
    void OnGunSlotAssign(ResourceInfo newGun)
    {
        GunResourceInfo gun = (GunResourceInfo)newGun;
        State.GunId = globalInfo.GetGunId(gun);
    }
}
