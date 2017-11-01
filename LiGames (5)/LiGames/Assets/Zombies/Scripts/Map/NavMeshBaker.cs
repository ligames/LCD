using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public void BakeAllSurfaces()
    {
        Bake(FindObjectsOfType<NavMeshSurface>());
    }
    public void Bake(NavMeshSurface[] surfaces)
    {
        foreach (NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }
}
