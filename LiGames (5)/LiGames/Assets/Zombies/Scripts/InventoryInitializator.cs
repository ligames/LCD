using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryInitializator : MonoBehaviour
{
    [Inject]
    BackpackContents backpack;
    [Inject]
    State state;
    [SerializeField]
    Transform inventoryGrid;
    [SerializeField]
    Transform personsGrid;
    [SerializeField]
    GameObject menu;
    [Inject]
    PersonManagmentUI personManagmentUI;
    [Inject]
    InventoryUIController uiController;

    List<SlotUI> slots;
    List<PersonSlotUI> personSlots;

    void Awake()
    {
        slots = new List<SlotUI>();
        for (int i = 0; i < inventoryGrid.childCount; ++i)
            slots.Add
            (
                inventoryGrid.GetChild(i).GetComponent<SlotUI>()
            );
        personSlots = new List<PersonSlotUI>();
        for (int i = 0; i < personsGrid.childCount; ++i)
            personSlots.Add
            (
                personsGrid.GetChild(i).GetComponent<PersonSlotUI>()
            );
    }
    bool inited = false;
    public void ShowMenu()
    {
        if (!inited)
            Init();
        menu.SetActive(true);
        uiController.OnShow();
    }
    void Init()
    {
        int i;
        for (i = 0; i < backpack.Resources.Count; ++i)
            slots[i].Assign(backpack.Resources[i]);
        for (; i < slots.Count; ++i)
            slots[i].Assign(null);
        for (int j = 0; j < state.Chars.Count; ++j)
            personSlots[j].Assign(state.Chars[j]);
        personManagmentUI.Assign(state.Chars[0]);
        inited = true;
    }
    public void HideMenu()
    {
        menu.SetActive(false);
    }
}
