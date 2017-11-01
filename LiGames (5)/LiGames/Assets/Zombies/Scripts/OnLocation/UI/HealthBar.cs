using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform bar;
    Vector3 cameraRotation;
    public float MaxHealth { get; set; }
    float maxBarScale;
    public float Health
    {
        set
        {
            bar.localScale = new Vector3
            (
                bar.localScale.x,
                value / MaxHealth * maxBarScale
            );
        }
    }

    void Awake()
    {
        cameraRotation = Camera.main.transform.rotation.eulerAngles;
        maxBarScale = bar.localScale.y;
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(cameraRotation);
    }
}
