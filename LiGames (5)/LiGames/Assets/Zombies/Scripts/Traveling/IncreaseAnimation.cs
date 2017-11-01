using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAnimation : MonoBehaviour
{
    [SerializeField] float startScale = 0.25f;
    [SerializeField] float speed = 0.8f;
    [SerializeField]
    float NormalScale = 1;
    List<RectTransform> animatedRects;

    //Dictionary<RectTransform, Coroutine> hidingCoroutines;
    //Dictionary<RectTransform, Coroutine> showingCoroutines;

    void Awake()
    {
        animatedRects = new List<RectTransform>();
        //hidingCoroutines = new Dictionary<RectTransform, Coroutine>();
        //showingCoroutines = new Dictionary<RectTransform, Coroutine>();
    }

    IEnumerator AnimateForward(RectTransform rect)
    {
        rect.localScale = new Vector3(startScale, startScale, startScale);
        rect.gameObject.SetActive(true);
        while (rect.localScale.x != NormalScale)
        {
            float nextScale = Mathf.Min(rect.localScale.x + speed * Time.deltaTime, NormalScale);
            rect.localScale = new Vector3(nextScale, nextScale, nextScale);
            yield return null;
        }
        //showingCoroutines.Remove(rect);
    }
    IEnumerator AnimateBack(RectTransform rect)
    {
        while (rect.localScale.x != startScale)
        {
            float nextScale = Mathf.Max(rect.localScale.x - speed * Time.deltaTime, startScale);
            rect.localScale = new Vector3(nextScale, nextScale, nextScale);
            yield return null;
        }
        rect.gameObject.SetActive(false);
        animatedRects.Remove(rect);
        //hidingCoroutines.Remove(rect);
    }
    public void ShowAnimation(List<RectTransform> rects)
    {
        //StopAllCoroutines();
        // animatedRects.AddRange(rects);
        animatedRects = rects;
        foreach (RectTransform rect in rects)
        {
            //if (hidingCoroutines.ContainsKey(rect))
            //{
            //    StopCoroutine(hidingCoroutines[rect]);
            //    hidingCoroutines.Remove(rect);
            //}
            Coroutine coroutine = StartCoroutine(AnimateForward(rect));
            //showingCoroutines.Add(rect, coroutine);
        }
    }
    public void HideElements()
    {
        //StopAllCoroutines();
        foreach (RectTransform rect in animatedRects)
        {
            Debug.Log("disable: " + rect);
            rect.gameObject.SetActive(false);
            //if(showingCoroutines.ContainsKey(rect))
            //{
            //    StopCoroutine(showingCoroutines[rect]);
            //    showingCoroutines.Remove(rect);
            //}
            //Coroutine coroutine = StartCoroutine(AnimateBack(rect));
            //hidingCoroutines.Add(rect, coroutine);
        }
    }
}
