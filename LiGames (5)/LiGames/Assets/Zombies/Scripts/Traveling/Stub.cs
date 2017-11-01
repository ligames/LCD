using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stub : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
