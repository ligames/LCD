using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonDescriptor : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    RawImage icon;
    [SerializeField]
    string personName;

    public GameObject Prefab { get { return prefab; } }
    public RawImage Icon { get { return icon; } }
    public string Name { get { return personName; } }
}
