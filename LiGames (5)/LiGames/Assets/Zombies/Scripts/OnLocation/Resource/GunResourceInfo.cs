using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunResourceInfo : ResourceInfo
{
    public GunTemplate template;
    public float constantDamage = 15.0f;
    public float distanceSensDamage = 50.0f;
    public override void Use(PersonActionController person)
    {
        person.Adopt(GetComponent<GunResourceInfo>());
    }
    public bool Equals(GunResourceInfo other)
    {
        if (!other)
            return false;
        return Name == other.Name;
    }
}
