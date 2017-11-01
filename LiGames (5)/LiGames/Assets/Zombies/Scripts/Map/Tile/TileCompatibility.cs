using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCompatibility : MonoBehaviour
{
    [SerializeField]
    List<int> leftCompatibleTiles;
    [SerializeField]
    List<int> rightCompatibleTiles;
    [SerializeField]
    List<int> upperCompatibleTiles;
    [SerializeField]
    List<int> bottomCompatibleTiles;

    public TileDescriptor GetLeftCompatibleTile()
    {
        TileDescriptor res = GetRandomCompatibleTile(leftCompatibleTiles);
        return res;
    }
    public TileDescriptor GetRightCompatibleTile()
    {
        TileDescriptor res = GetRandomCompatibleTile(rightCompatibleTiles);
        return res;
    }
    public TileDescriptor GetUpperCompatibleTile()
    {
        TileDescriptor res = GetRandomCompatibleTile(upperCompatibleTiles);
        return res;
    }
    public TileDescriptor GetBottomCompatibleTile()
    {
        TileDescriptor res = GetRandomCompatibleTile(bottomCompatibleTiles);
        return res;
    }

    TileDescriptor GetRandomCompatibleTile(List<int> possibleTiles)
    {
        GlobalInfoAccessor globalAccessor = FindObjectOfType<GlobalInfoAccessor>();
        int randomIndex = possibleTiles[Random.Range(0, possibleTiles.Count)];
        TileDescriptor res = globalAccessor.TilesPrefabs[randomIndex].GetComponent<TileDescriptor>();
        return res;
    }
}
