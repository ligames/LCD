using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SlotUI : MonoBehaviour
{
    [Inject]
    InventoryUIController inventoryController;
    
    [SerializeField]
    ResourceInfo item;
    int index;
    [SerializeField]
    RawImage image;

    public event System.Action<ResourceInfo> onAssign;
    public ResourceInfo Item { get { return item; } }
    public int Index { get { return index; } }

    void Awake()
    {
        Iinitialize(0, item);
    }

    public void Iinitialize(int i, ResourceInfo res)
    {
        index = i;
        Assign(res);
    }
    public void Assign(ResourceInfo newItem)
    {
        item = newItem;
        UpdateImage();
        if (onAssign != null)
            onAssign(item);
    }
    public void Swap(SlotUI swapSlot)
    {
        if (this == swapSlot)
            return;
        ResourceInfo temp = swapSlot.Item;
        swapSlot.Assign(Item);
        Assign(temp);
    }
    void UpdateImage()
    {
        if (Item)
            image.texture = Item.image.texture;
        image.gameObject.SetActive(Item);
    }
    public virtual bool IsCompatibleWith(SlotUI other)
    {
        return true;
    }
}
