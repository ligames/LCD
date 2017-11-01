using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDescriptor : MonoBehaviour
{
    [SerializeField]
    Transform locationsContainer;
    [SerializeField]
    BorderLocation leftBorder, rightBorder, upperBorder, bottomBorder;
    [SerializeField]
    TileCompatibility compatibility;

    List<Location> locations;
    List<Location> allLocations;

    void Awake()
    {
        GetLocations();
        allLocations = new List<Location>();
        allLocations.AddRange(locations);
        foreach (Location borderLocation in new List<Location>
        {
            leftBorder,
            rightBorder,
            upperBorder,
            bottomBorder
        })
            if (!allLocations.Contains(borderLocation))
                allLocations.Add(borderLocation);
    }

    public BorderLocation Left
    {
        get { return leftBorder; }
    }
    public BorderLocation Right
    {
        get { return rightBorder; }
    }
    public BorderLocation Upper
    {
        get { return upperBorder; }
    }
    public BorderLocation Bottom
    {
        get { return bottomBorder; }
    }

    public Location this[int locationIndex]
    {
        get { return locations[locationIndex]; }
    }

    public TileDescriptor GetLeftCompatibleTile()
    {
        return compatibility.GetLeftCompatibleTile();
    }
    public TileDescriptor GetRightCompatibleTile()
    {
        return compatibility.GetRightCompatibleTile();
    }
    public TileDescriptor GetUpperCompatibleTile()
    {
        return compatibility.GetUpperCompatibleTile();
    }
    public TileDescriptor GetBottomCompatibleTile()
    {
        return compatibility.GetBottomCompatibleTile();
    }
    public Location RandomLocation
    {
        get
        {
            return allLocations[11];
            System.Random numberGenerator = new System.Random();
            int randomIndex = numberGenerator.Next(allLocations.Count);
            return allLocations[randomIndex];
        }
    }

    public void InitializeLocations(int x, int y)
    {
        for (int i = 0; i < allLocations.Count; ++i)
            allLocations[i].Initialize(x, y, i);
    }
    public void InitializeBorderLocations(int x, int y)
    {
        for (int i = 0; i < allLocations.Count; ++i)
            if (allLocations[i].GetType() == typeof(BorderLocation))
                allLocations[i].Initialize(x, y, i);
    }
    public bool ContainLocation(Location loc)
    {
        return allLocations.Contains(loc);
    }
    
    void GetLocations()
    {
        locations = new List<Location>();
        for (int i = 0; i < locationsContainer.childCount; ++i)
            locations.Add
                (
                    locationsContainer.GetChild(i)
                        .GetComponent<Location>()
                );
    }
    public int IndexOfLocation(Location loc)
    {
        int index = -1;
        for(int i = 0; i < allLocations.Count; ++i)
            if(loc == allLocations[i])
            {
                index = i;
                break;
            }
        return index;
    }
    public void Save(int x, int y)
    {
        foreach(Location location in allLocations)
        {
            PlayerPrefs.SetInt
                (
                    SavingHelper.XCoordinate + x + SavingHelper.YCoordinate + y +
                    SavingHelper.Tile + location.LocationIndex + SavingHelper.Location + SavingHelper.IsSaved, 
                    1
                );
            PlayerPrefs.SetInt
                (
                    SavingHelper.XCoordinate + x + SavingHelper.YCoordinate + y +
                    SavingHelper.Tile + location.LocationIndex + SavingHelper.Location, 
                    location.Id
                );
        }
    }
    public bool Equals(TileDescriptor other)
    {
        return locationsContainer == other.locationsContainer &&
            leftBorder == other.leftBorder;
    }
}
