using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceInfo : MonoBehaviour
{
    public GameObject resourcePrefab;
    public GameObject cleanPrefab;
    public GameObject resourceIcon;
    public RawImage image;
    public bool expendable = false;

    public string Name = "default";
    public virtual void Use(PersonActionController person)
    {
        print(Name);
    }
    public bool IsEquals(ResourceInfo other)
    {
        return resourcePrefab == other.resourcePrefab &&
            cleanPrefab == other.cleanPrefab &&
            resourceIcon == other.resourceIcon &&
            expendable == other.expendable &&
            Name == other.Name;
    }
}
