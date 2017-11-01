using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Illumination : MonoBehaviour
{
    [Inject]
    Field heightHandler;
    public GameObject mat;
    public GameObject colliderHandler;

    public bool IsOn { get; private set; }

    void Awake()
    {
        Renderer matRenderer = mat.GetComponent<MeshRenderer>();
        Vector3 multiplicator = new Vector3();
        mat.transform.rotation = colliderHandler.transform.rotation;
        Bounds bounds = colliderHandler.GetComponent<BoxCollider>().bounds;
        multiplicator.x = matRenderer.bounds.size.x / mat.transform.localScale.x;
        multiplicator.z = matRenderer.bounds.size.z / mat.transform.localScale.z;
        mat.transform.localScale = new Vector3(
            bounds.size.x / multiplicator.x,
            mat.transform.localScale.y,
            bounds.size.z / multiplicator.z
        );
        mat.transform.position = new Vector3(
            colliderHandler.transform.position.x,
            heightHandler.Height,
            colliderHandler.transform.position.z
        );
    }

    #region API
    public void On()
    {
        mat.SetActive(true);
        IsOn = true;
    }
    public void Off()
    {
        mat.SetActive(false);
        IsOn = false;
    }
    #endregion
}
