using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderLocation : Location
{
    [SerializeField]
    float borderDistance;

    public float BorderDistance { get { return borderDistance; } }

    public void Bind(BorderLocation bLocation)
    {
        possiblePaths.Add
            (new Path
                (
                    bLocation,
                    DistanceTo(bLocation)
                )
            );
    }
    public float DistanceTo(BorderLocation other)
    {
        return other.BorderDistance + BorderDistance;
    }
}
