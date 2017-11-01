using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePhysic : MonoBehaviour
{
    public ResourceInfo info;
    void OnTriggerEnter(Collider other)
    {
        PersonActionController person = other.gameObject.GetComponent<PersonActionController>();
        if (person)
        {
            Vector3 shift = GameObject.FindObjectOfType<GameInfo>().ItemToIconShift;
            ResourceIcon icon = Instantiate(info.resourceIcon).GetComponent<ResourceIcon>();

            icon.Info = info;
            icon.Person = person;
            icon.transform.position = transform.position + shift;

            Destroy(gameObject);
        }
    }
}
