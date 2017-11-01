using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSlotUI : SlotUI
{
    public override bool IsCompatibleWith(SlotUI other)
    {
        GunResourceInfo gunInfo;
        try
        {
            gunInfo = (GunResourceInfo)other.Item;
        }
        catch
        {
            return false;
        }
        return true;
    }
}
