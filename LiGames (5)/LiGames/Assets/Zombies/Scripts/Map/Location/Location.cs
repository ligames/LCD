using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField]
    Transform carMount;
    [SerializeField]
    protected List<Path> possiblePaths;
    [SerializeField]
    Transform skinMount;

    int X { get; set; }
    int Y { get; set; }
    public int LocationIndex { get; private set; }
    public int Id { get; set; }

    public LocationDescriptor Descriptor { get; private set; }
    public Transform CarMount { get { return carMount; } }
    public Transform SkinMount { get { return skinMount; } }
    MapGenerator generator;
    MapGenerator Generator
    {
        get
        {
            if (!generator)
                generator = FindObjectOfType<MapGenerator>();
            return generator;
        }
    }

    bool IsLocationExist
    {
        get
        {
            bool isTileExists = PlayerPrefs.GetInt
                (
                    SavingHelper.XCoordinate + X + SavingHelper.YCoordinate + Y
                ) > 0;
            return SavingHelper.IsGameSaved && isTileExists;
        }
    }
    public void Initialize(int x, int y, int locationIndex)
    {
        X = x;
        Y = y;
        LocationIndex = locationIndex;

        InitializeDescriptor();
        GameObject skin = Instantiate(Descriptor.Skin, skinMount);
        //skin.transform.position = skinMount.position;
    }

    public List<Path> GetNeighbours()
    {
        Path[] res = new Path[possiblePaths.Count];
        possiblePaths.CopyTo(res);
        return new List<Path>(res);
    }
    void InitializeDescriptor()
    {
        int descriptorIndex;
        if (!IsLocationExist)
            Descriptor = Generator.GenLocationDescriptor(out descriptorIndex);
        else
        {
            descriptorIndex = PlayerPrefs.GetInt
            (
                SavingHelper.XCoordinate + X + SavingHelper.YCoordinate + Y +
                SavingHelper.Tile + LocationIndex + SavingHelper.Location
            );
            Descriptor = Generator.GetLocationDescriptor(descriptorIndex);
        }
        Id = descriptorIndex;
    }
    public float DistanceTo(Location other)
    {
        float distance = float.MaxValue;
        foreach(Path path in possiblePaths)
            if(path.Destination == other)
            {
                distance = path.Distance;
                break;
            }
        return distance;
    }
}
