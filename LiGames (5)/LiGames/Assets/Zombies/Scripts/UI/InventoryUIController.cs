using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.EventSystems;

public class InventoryUIController : MonoBehaviour
{
    SlotUI floatingSlot;
    SlotUI slotUnderPointer;
    [SerializeField]
    RawImage slotIcon;
    [Inject]
    GlobalInfo globalInfo;
    [Inject]
    BackpackContents backPack;
    ITap tapInput;

    void Awake()
    {
        tapInput = globalInfo.TapInput;
    }

    public void OnShow()
    {
        StartCoroutine(PointerObserving());
        StartCoroutine(IconChasePointer());
    }
    public void OnSlotDown(SlotUI slot)
    {
        if (!slot.Item)
            return;
        floatingSlot = slot;
        SetChasingIconActive(true);
        UpdateChasingIconTexture();
    }
    public void OnSlotEnter(SlotUI slot)
    {
        if (!floatingSlot)
            return;
        slotUnderPointer = slot;
    }
    public void OnSlotExit(SlotUI slot)
    {
        slotUnderPointer = null;
    }
    void OnTouchUp()
    {
        StopChasePointer();
        if (!slotUnderPointer)
            return;
        if (floatingSlot.IsCompatibleWith(slotUnderPointer) && 
            slotUnderPointer.IsCompatibleWith(floatingSlot))
        {
            if(floatingSlot.GetType() != slotUnderPointer.GetType())
            {
                OnSwapItem(floatingSlot.Item, slotUnderPointer.Item);
            }
            floatingSlot.Swap(slotUnderPointer);
        }
    }
    IEnumerator PointerObserving()
    {
        while (true)
        {
            if (tapInput.GetTapUp() != null)
                OnTouchUp();
            yield return null;
        }
    }
    IEnumerator IconChasePointer()
    {
        while(true)
        {
            UpdateChasingIconPosition();
            yield return null;
        }
    }
    void StopChasePointer()
    {
        SetChasingIconActive(false);
    }
    void SetChasingIconActive(bool val)
    {
        Color newColor = slotIcon.color;
        newColor.a = val ? 1 : 0;
        slotIcon.color = newColor;
    }
    void UpdateChasingIconPosition()
    {
        if (tapInput.CurrentPosition != null)
            slotIcon.rectTransform.position = 
                tapInput.CurrentPosition.Position;
    }
    void UpdateChasingIconTexture()
    {
        if (floatingSlot)
            slotIcon.texture = floatingSlot.Item.image.texture;
    }
    public void OnSwapItem(ResourceInfo item, ResourceInfo toItem)
    {
        backPack.RemoveItem(item);
        backPack.AddItem(toItem);
    }
}
