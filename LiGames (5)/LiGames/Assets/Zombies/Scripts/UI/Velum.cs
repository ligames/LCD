using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Velum : MonoBehaviour
{
    [Inject]
    GlobalInfo globalInfo;
    [SerializeField]
    Image panel;
    float speed;
    void Start()
    {
        speed = 1.0f / globalInfo.VelumProcessingTime;
        StartCoroutine(Open());
    }
    IEnumerator Open()
    {
        panel.gameObject.SetActive(true);
        yield return Processing(-1, 0f);
        panel.gameObject.SetActive(false);
    }
    public IEnumerator Close()
    {
        panel.gameObject.SetActive(true);
        yield return Processing(1, 1f);
    }
    IEnumerator Processing(int sign, float target)
    {
        while (panel.color.a != target)
        {
            Color newColor = panel.color;
            newColor.a = Mathf.Clamp
            (
                newColor.a + speed * Time.deltaTime * sign,
                0f,
                1f
            );
            panel.color = newColor;
            yield return null;
        }
    }
}
