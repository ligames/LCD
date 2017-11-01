using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPerformanceImprovable
{
    void ImprovePerformance();
}

public class MapPerformanceImprover : MonoBehaviour, IPerformanceImprovable
{
    public void ImprovePerformance()
    {
        foreach(TileDescriptor tile in FindObjectsOfType<TileDescriptor>())
        {
            foreach (MeshRenderer tileRenderer in
                tile.GetComponentsInChildren(typeof(MeshRenderer), true))
                if (!tileRenderer.isVisible)
                    tileRenderer.gameObject.SetActive(false);
        }
    }
}
