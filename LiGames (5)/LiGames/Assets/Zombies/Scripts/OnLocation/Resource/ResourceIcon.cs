using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIcon : MonoBehaviour
{
    public ResourceInfo Info { get; set; }
    public PersonActionController Person { get; set; }

    GameInfo gameInfo;
    BackpackContents backPack;

    float speed;
    void Awake()
    {
        gameInfo = FindObjectOfType<GameInfo>();
        backPack = FindObjectOfType<BackpackContents>();
        Vector3 pos = Camera.main.ScreenToWorldPoint(gameInfo.BackpackTransform.position);
        StartCoroutine(BackPackMoving(pos));
    }
    IEnumerator BackPackMoving(Vector3 position)
    {
        speed = gameInfo.resourceIconSpeed * Time.deltaTime ;
        while (transform.position != position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, speed);
            speed += gameInfo.resourceAcceleration * Time.deltaTime;
            yield return null;
        }
        backPack.AddItem(Info, Person);
        Destroy(gameObject);
    }
}
