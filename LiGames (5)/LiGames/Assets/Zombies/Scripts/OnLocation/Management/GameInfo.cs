using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Zenject;

public class GameInfo : MonoBehaviour
{
    public Vector3 ItemToIconShift = new Vector3(0.6f, -0.8f, 0.8f);
    [SerializeField] Transform backpackTransform;
    public Transform BackpackTransform { get { return backpackTransform; } }
    public float resourceIconSpeed = 10f;
    public float resourceAcceleration = 5f;

    public float radiusError = 0.1f;
    public float rotationError = 0.2f;

    public Vector3 gunRotation;
    public Vector3 gunPosition;

    public static string terrainTag = "Terrain";
    static PersonObserver observer;

    public static GameObject GetObjectByName (Transform t, string name)
    {
        GameObject res = null;
        if (t.name == name)
            res = t.gameObject;
        else
            for (int i = 0; i < t.childCount; ++i)
                if ((res = GetObjectByName(t.GetChild(i), name)) && res.name == name)
                    break;
        return res;
    }
    public static bool IsTapOnUI(Vector3 pos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
    public static PersonObserver GetPersonObserver()
    {
        if (!observer)
            observer = FindObjectOfType<PersonObserver>();
        return observer;
    }
}
