using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationDescriptor : MonoBehaviour
{
    [SerializeField]
    int locationSceneIndex;
    [SerializeField]
    GameObject locationSkin;

    public GameObject Skin { get { return locationSkin; } }

    public void EnterIntoLocation()
    {
        SceneManager.LoadScene(locationSceneIndex);
    }
}
