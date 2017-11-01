using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ResourceManager : MonoBehaviour
{
    public GameObject grid;
    public GameObject menu;

    [Inject]
    BackpackContents backPack;

    List<CellDescriptor> gridCells;

    void Awake()
    {
        gridCells = new List<CellDescriptor>();
        foreach (CellDescriptor cellD in grid.GetComponentsInChildren(typeof(CellDescriptor), true))
        {
            gridCells.Add(cellD);
            cellD.onClick += UseBackPackItem;
        }      
    }
    void UseBackPackItem(ResourceInfo resource)
    {
        Debug.Log("Using item");
        //resource.Use(CurrentPerson);
        if (resource.expendable)
            backPack.RemoveItem(resource);
        HideMenu();
    }
    #region API
    public void ShowMenu()
    {
        foreach (CellDescriptor cellD in gridCells)
            cellD.Clear();
        for (int i = 0; i < backPack.Resources.Count; ++i)
            gridCells[i].MountResourceInfo(backPack.Resources[i]);
        menu.SetActive(true);
    }
    public void HideMenu()
    {
        menu.SetActive(false);
    }
    #endregion
}
