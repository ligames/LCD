using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int Y { get; set; }
    public int X { get; set; }
    public Tile Left { get; set; }
    public Tile Right { get; set; }
    public Tile Upper { get; set; }
    public Tile Bottom { get; set; }
    public int Id { get; set; }
    public TileDescriptor Descriptor { get; set; }
    MeshRenderer meshRenderer;

    public void BindToExternalTiles()
    {
        Descriptor.Left.Bind
            (
                Left.Descriptor.Right
            );
        Descriptor.Right.Bind
            (
                Right.Descriptor.Left
            );
        Descriptor.Upper.Bind
            (
                Upper.Descriptor.Bottom
            );
        Descriptor.Bottom.Bind
            (
                Bottom.Descriptor.Upper
            );
    }

    public Vector3 TileRightNeighbourPosition
    {
        get { return Descriptor.transform.position + TileRightNeighbourOffset; }
    }

    public Vector3 TileLeftNeighbourPosition
    {
        get { return Descriptor.transform.position - TileRightNeighbourOffset; }
    }

    public Vector3 TileUpperNeighbourPosition
    {
        get { return Descriptor.transform.position + TileUpperNeighbourOffset; }
    }

    public Vector3 TileBottomNeighbourPosition
    {
        get { return Descriptor.transform.position - TileUpperNeighbourOffset; }
    }

    Vector3 TileRightNeighbourOffset
    {
        get
        { return new Vector3(MeshRendererBounds.size.x, 0f, 0f); }
    }

    Vector3 TileUpperNeighbourOffset
    {
        get
        { return new Vector3(0f, 0f, MeshRendererBounds.size.z); }
    }

    Bounds meshBounds;
    bool isMeshBoundsCalculated = false;
    Bounds MeshRendererBounds
    {
        get
        {
            if (!isMeshBoundsCalculated)
            {
                //meshBounds = Descriptor.GetComponent<Renderer>().bounds;
                foreach (Renderer rend in Descriptor.GetComponentsInChildren(typeof(Renderer), true))
                    meshBounds.Encapsulate(rend.bounds);
                meshBounds = Descriptor.GetComponent<BoxCollider>().bounds;
            }
            return meshBounds;
        }
    }
    public void Save()
    {
        PlayerPrefs.SetInt(SavingHelper.XCoordinate + X + SavingHelper.YCoordinate + Y, 1); // tile exists
        PlayerPrefs.SetInt
            (
                SavingHelper.XCoordinate + X + SavingHelper.YCoordinate + Y + SavingHelper.Tile,
                Id
            );
        Descriptor.Save(X, Y);
    }
}
