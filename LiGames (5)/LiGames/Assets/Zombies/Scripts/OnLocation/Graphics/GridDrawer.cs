using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GridDrawer : MonoBehaviour
{
    [Inject]
    Field field;

    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        Vector3[] gridPositions = DesignGrid(field.Center, field.FieldWidth, field.FieldHeight, field.CellSize);
        lineRenderer.positionCount = gridPositions.Length;
        lineRenderer.SetPositions(gridPositions);
    }

    Vector3[] DesignGrid(Vector3 center, float width, float height, float cellsize)
    {
        List<Vector3> gridPositions = new List<Vector3>();

        Vector3 carriagePosition = new Vector3(
            center.x - width / 2.0f,
            center.y + 0.008f,
            center.z - height / 2.0f
        );

        int upDownMult = -1;

        gridPositions.Add(carriagePosition);
        carriagePosition.z += height;
        gridPositions.Add(carriagePosition);

        while(carriagePosition.x + cellsize < center.x + width / 2.0f)
        {
            carriagePosition.x += cellsize;
            gridPositions.Add(carriagePosition);
            carriagePosition.z += height * upDownMult;
            gridPositions.Add(carriagePosition);

            upDownMult *= -1;
        }

        int leftRightMult = 1;
        if (upDownMult < 0)
        {
            carriagePosition.z -= height;
            gridPositions.Add(carriagePosition);
        }

        carriagePosition.x -= width;
        gridPositions.Add(carriagePosition);

        while(carriagePosition.z + cellsize < center.z + height / 2.0f)
        {
            carriagePosition.z += cellsize;
            gridPositions.Add(carriagePosition);
            carriagePosition.x += width * leftRightMult;
            gridPositions.Add(carriagePosition);

            leftRightMult *= -1;
        }
        

        return gridPositions.ToArray();
    }
}
