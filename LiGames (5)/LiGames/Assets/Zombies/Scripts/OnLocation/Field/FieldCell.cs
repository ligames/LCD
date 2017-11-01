using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCell : MonoBehaviour
{
    MeshRenderer mRend;
    public bool IsPossibleToStandOn { get; private set; }
    void Awake()
    {
        mRend = GetComponent<MeshRenderer>();
        IsPossibleToStandOn = true;
    }
    public int I { get; set; }
    public int J { get; set; }
    #region API
    public void Activate()
    {
        mRend.enabled = true;
    }
    public void Disactivate()
    {
        mRend.enabled = false;
    }
    public void CheckIfPossibleToStandOn(int layerMask, string tag, float distance)
    {
        Ray ray = new Ray(
            new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z),
            transform.up * distance
             );
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {
            //print("hit");
            foreach(RaycastHit hit in hits)
                if (hit.collider.gameObject.tag == tag)
                {
                    IsPossibleToStandOn = false;
                    return;
                }
        }
    }
    #endregion
}
