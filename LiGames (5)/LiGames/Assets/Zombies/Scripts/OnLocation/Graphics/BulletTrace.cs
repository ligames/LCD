using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrace : MonoBehaviour
{
    Vector3 destination;
    public float speed;
    public Vector3 Destination
    {
        get { return destination; }
        set
        {
            destination = new Vector3(
                value.x,
                transform.position.y,
                value.z);
            StartCoroutine("Motion");
        }
    }
    IEnumerator Motion()
    {
        while(transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
