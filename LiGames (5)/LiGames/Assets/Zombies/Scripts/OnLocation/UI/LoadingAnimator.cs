using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimator : MonoBehaviour
{
    [SerializeField]
    RectTransform circleImageTransform;
    [SerializeField]
    float rotatingSpeed = 12f;
    void Awake()
    {
        circleImageTransform.gameObject.SetActive(false);
    }
    public void ShowLoading()
    {
        circleImageTransform.gameObject.SetActive(true);
        StartCoroutine(Rotating());
    }
    public void StopLoading()
    {
        circleImageTransform.gameObject.SetActive(false);
        StopAllCoroutines();
    }
    IEnumerator Rotating()
    {
        while(true)
        {
            circleImageTransform.Rotate(0f, 0f, rotatingSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
