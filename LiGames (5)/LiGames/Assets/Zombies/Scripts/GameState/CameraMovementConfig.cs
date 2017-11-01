using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CameraMotionConfig", menuName ="Camera motion config")]
public class CameraMovementConfig : ScriptableObject
{
    [SerializeField]
    float deltaTime = 0.3f;

    public float DeltaTime { get { return deltaTime; } }
}
