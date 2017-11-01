using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MapGenerator : SaveableBehavior
{
    [Inject]
    GlobalInfoAccessor globalInfo;
    [SerializeField]
    Transform locationDescriptorsContainer;
    List<LocationDescriptor> locationDescriptors;
    Location currentLocation;

    Tile centerTile;
    public TileDescriptor Center { get; private set; }
    delegate TileDescriptor TileDescriptorHandler();
    List<Tile> tiles;

    new void Awake()
    {
        base.Awake();
        tiles = new List<Tile>();
        InitLocDescriptorsPrefabs();
    }
    
    public void CreateWorld()
    {
        centerTile = GetCenterTile();
        Center = centerTile.Descriptor;
        tiles.Add(centerTile);
        InitTilesAroundTile(centerTile);
        //foreach (Tile tile in tiles)
        //    InitTilesLocations(tile);
        InitTilesLocations(centerTile);
        foreach (Tile tile in tiles)
            if (tile != centerTile)
                tile.Descriptor.InitializeBorderLocations(tile.X, tile.Y);
    }
    public Location GenerateRandomLocation()
    {
        return Center.RandomLocation;
    }
    public void BindCenterToEdges()
    {
        centerTile.BindToExternalTiles();
    }
    public void UpdateCenter(Location loc)
    {
        currentLocation = loc;
        centerTile = GetTileByLocation(currentLocation);
    }
    Tile GetCenterTile()
    {
        Tile tile = new Tile();
        GameObject tileGO;
        if (!SavingHelper.IsGameSaved)
        {
            tile.X = tile.Y = 0;
            System.Random numberGenerator = new System.Random();
            int randomIndex = numberGenerator.Next(globalInfo.TilesPrefabs.Count);
            tileGO = Instantiate(globalInfo.TilesPrefabs[randomIndex].gameObject);
            tile.Id = randomIndex;
        }
        else
        {
            tile.X = PlayerPrefs.GetInt(SavingHelper.TileCenter + SavingHelper.XCoordinate);
            tile.Y = PlayerPrefs.GetInt(SavingHelper.TileCenter + SavingHelper.YCoordinate);
            int tileGOIndex = PlayerPrefs.GetInt(
                SavingHelper.XCoordinate + tile.X + SavingHelper.YCoordinate + tile.Y +
                SavingHelper.Tile);
            tile.Id = tileGOIndex;
            tileGO = Instantiate(globalInfo.TilesPrefabs[tileGOIndex].gameObject);
        }
        tile.Descriptor = tileGO.GetComponent<TileDescriptor>();
        return tile;
    }

    void InitTilesAroundTile(Tile tile)
    {
        InitRightAndLeftTiles(tile);
        InitUpperAndBottomTiles(tile);
        InitUpperAndBottomTiles(tile.Right);
        InitUpperAndBottomTiles(tile.Left);
    }
    void InitRightAndLeftTiles(Tile tile)
    {
        Tile rightTile = GetTileByCoordinates
        (
            tile.X + 1,
            tile.Y,
            tile.Descriptor.GetRightCompatibleTile
        );
        Tile leftTile = GetTileByCoordinates
        (
            tile.X - 1,
            tile.Y,
            tile.Descriptor.GetLeftCompatibleTile
        );
        rightTile.Descriptor.transform.position = tile.TileRightNeighbourPosition;
        leftTile.Descriptor.transform.position = tile.TileLeftNeighbourPosition;
        tile.Right = rightTile;
        tile.Left = leftTile;
        tiles.Add(rightTile);
        tiles.Add(leftTile);
    }
    void InitUpperAndBottomTiles(Tile tile)
    {
        Tile upperTile = GetTileByCoordinates
            (
                tile.X,
                tile.Y + 1,
                tile.Descriptor.GetUpperCompatibleTile
            );
        Tile bottomTile = GetTileByCoordinates
             (
                 tile.X,
                 tile.Y - 1,
                 tile.Descriptor.GetBottomCompatibleTile
             );
        upperTile.Descriptor.transform.position = tile.TileUpperNeighbourPosition;
        bottomTile.Descriptor.transform.position = tile.TileBottomNeighbourPosition;
        tile.Upper = upperTile;
        tile.Bottom = bottomTile;
        tiles.Add(upperTile);
        tiles.Add(bottomTile);
    }

    Tile GetTileByCoordinates(int x, int y, TileDescriptorHandler getCompatibleTile)
    {
        GameObject tileGO;
        GameObject prefab;
        Tile tile = new Tile();
        int index;
        tile.X = x;
        tile.Y = y;

        if (!IsTileExists(tile))
        {
            prefab = getCompatibleTile().gameObject;
            index = globalInfo.GetTileId(prefab);// GetTileIndex(prefab);
        }
        else
        {
            index = PlayerPrefs.GetInt
                (
                    SavingHelper.XCoordinate + x + SavingHelper.YCoordinate + y + SavingHelper.Tile
                );
            prefab = globalInfo.TilesPrefabs[index];
        }
        tile.Id = index;
        tileGO = Instantiate(prefab).gameObject;
        tile.Descriptor = tileGO.GetComponent<TileDescriptor>();
        return tile;
    }

    bool IsTileExists(Tile tile)
    {
        if (!SavingHelper.IsGameSaved)
            return false;
        return PlayerPrefs.GetInt(SavingHelper.XCoordinate + tile.X + SavingHelper.YCoordinate + tile.Y) > 0;
    }

    void InitTilesLocations(Tile tile)
    {
        tile.Descriptor.InitializeLocations(tile.X, tile.Y);
    }

    public LocationDescriptor GenLocationDescriptor(out int index)
    {
        System.Random numberGenerator = new System.Random();
        index = numberGenerator.Next(locationDescriptors.Count);
        return locationDescriptors[index];
    }
    public LocationDescriptor GetLocationDescriptor(int index)
    {
        return locationDescriptors[index];
    }

    void InitLocDescriptorsPrefabs()
    {
        locationDescriptors = new List<LocationDescriptor>();
        for (int i = 0; i < locationDescriptorsContainer.childCount; ++i)
            locationDescriptors.Add
                (
                    locationDescriptorsContainer.GetChild(i).
                        GetComponent<LocationDescriptor>()
                );
    }
    public override void Save()
    {
        if (centerTile == null)
            return;
        PlayerPrefs.SetInt(SavingHelper.TileCenter + SavingHelper.XCoordinate, centerTile.X);
        PlayerPrefs.SetInt(SavingHelper.TileCenter + SavingHelper.YCoordinate, centerTile.Y);
        foreach (Tile tile in tiles)
        {
            tile.Save();
        }

        PlayerPrefs.SetInt
            (
                SavingHelper.XCoordinate + centerTile.X + SavingHelper.YCoordinate + centerTile.Y +
                SavingHelper.Tile + SavingHelper.Location,
                centerTile.Descriptor.IndexOfLocation(currentLocation)
            );
    }
    Tile GetTileByLocation(Location loc)
    {
        Tile res = null;
        foreach (Tile tile in tiles)
            if (tile.Descriptor.ContainLocation(loc))
            {
                res = tile;
                break;
            }
        return res;
    }
}
