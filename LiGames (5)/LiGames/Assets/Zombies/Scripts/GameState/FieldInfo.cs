using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Field config", menuName ="Field config")]
public class FieldInfo : ScriptableObject
{
    [SerializeField]
    int impossibleMoveLayerNumber = 9;
    [SerializeField]
    string impossibleMoveTag = "MoveImpossible";
    [SerializeField]
    float impossibleMoveCheckDistance = 10f;

    public int ImpossibleMoveLayerNumber { get { return impossibleMoveLayerNumber; } }
    public string ImpossibleMoveTag { get { return impossibleMoveTag; } }
    public float ImpossibleMoveCheckDistance { get { return impossibleMoveCheckDistance; } }
}
