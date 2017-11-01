using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}


class PersonsPositionsDispatcher
{
    Dictionary<Person, FieldCell> personsNearestCells;
    List<FieldCell> stakedOutCells;
    public Dictionary<float, List<Point>> OffsetMasks { get; private set; }
    public float CellSize { get; set; }
    public PersonsPositionsDispatcher()
    {
        personsNearestCells = new Dictionary<Person, FieldCell>();
        stakedOutCells = new List<FieldCell>();
        OffsetMasks = new Dictionary<float, List<Point>>();
    }
    public void RegisterNewPersonPosition(Person p, FieldCell cell)
    {
        if (personsNearestCells.ContainsKey(p))
            personsNearestCells[p] = cell;
        else personsNearestCells.Add(p, cell);

        float radius = p.radiusInCells * CellSize;
        if (!OffsetMasks.ContainsKey(radius))
            CalculateMask(radius);
    }
    public void RemovePerson(Person p)
    {
        personsNearestCells.Remove(p);
    }
    public FieldCell GetPersonsPosition(Person p)
    {
        return personsNearestCells[p];
    }
    public bool IsAnyPersonOnCell(FieldCell cell)
    {
        foreach (Person p in personsNearestCells.Keys)
            if (personsNearestCells[p] == cell)
                return true;
        return false;
    }
    public ZombieController GetEnemyOnCell(FieldCell cell)
    {
        foreach (Person person in personsNearestCells.Keys)
        {
            ZombieController enemy;
            try
            {
                enemy = (ZombieController)person;
            }
            catch { continue; }
            if (personsNearestCells[enemy] == cell)
                return enemy;
        }
        return null;
    }
    public bool IsCellStakedOut(FieldCell cell)
    {
        foreach (FieldCell c in stakedOutCells)
            if (c == cell)
                return true;
        return false;
    }
    public void StakeOutCell(FieldCell cell)
    {
        stakedOutCells.Add(cell);
    }
    public void FreeStakedOutCells()
    {
        stakedOutCells.Clear();
    }
    void CalculateMask(float radius)
    {
        int maxCells = (int)(radius / CellSize);
        List<Point> offsets = new List<Point>();
        for(int x = -maxCells; x <= maxCells; ++x)
        {
            for(int y = -maxCells; y <= maxCells; ++y)
            {
                if (Mathf.Sqrt(
                    Mathf.Pow(Mathf.Abs(x) * CellSize, 2.0f) +
                    Mathf.Pow(Mathf.Abs(y) * CellSize, 2.0f))
                    <= radius)
                offsets.Add(new Point { X = x, Y = y });
            }
        }
        OffsetMasks.Add(radius, offsets);
    }
}

public class Field : MonoBehaviour
{
    [Inject]
    GlobalInfo globalInfo;
    [Inject]
    FieldInfo fieldInfo;

    PersonsPositionsDispatcher dispatcher;
    FieldCell[,] cells;

    public float CellSize { get; private set; }
    public float FieldWidth { get; private set; }
    public float FieldHeight { get; private set; }
    public Vector3 Center { get; private set; }
    public float Height { get; private set; }

    public List<FieldCell> activeCells { get; private set; }

    bool fieldBuilt = false;

    void Awake()
    {
        activeCells = new List<FieldCell>();
        CorrectItemsPositions();
    }
    void BuildField()
    {
        //Bounds planeBounds = GetComponent<Renderer>().bounds;
        Bounds planeBounds = GetComponent<Renderer>().bounds;
        FieldWidth = planeBounds.size.x;
        FieldHeight = planeBounds.size.z;
        CellSize = globalInfo.CellPrefab.GetComponent<Renderer>().bounds.size.x;
        dispatcher = new PersonsPositionsDispatcher();
        dispatcher.CellSize = CellSize;
        Center = planeBounds.center;
        Height = planeBounds.center.y;

        cells = new FieldCell[
            (int)(FieldHeight / CellSize),
            (int)(FieldWidth / CellSize)];

        //Vector3 startPos = new Vector3(
        //    transform.position.x - FieldWidth / 2.0f + CellSize / 2.0f,
        //    transform.position.y + 0.001f,
        //    transform.position.z - FieldHeight / 2.0f + CellSize / 2.0f
        //);

        Vector3 startPos = new Vector3(
            Center.x - FieldWidth / 2.0f + CellSize / 2.0f,
            Center.y + 0.001f,
            Center.z - FieldHeight / 2.0f + CellSize / 2.0f
        );

        int layerMask =  1 << (fieldInfo.ImpossibleMoveLayerNumber);
        for(int i = 0; i < cells.GetLength(0); ++i)
        {
            Vector3 rowPos = new Vector3(
                startPos.x,
                startPos.y,
                startPos.z + i * CellSize);
            for (int j = 0; j < cells.GetLength(1); ++j)
            {
                GameObject c = Instantiate(globalInfo.CellPrefab);

                c.transform.parent = transform;
                c.transform.position = new Vector3(
                    rowPos.x + j * CellSize,
                    rowPos.y,
                    rowPos.z);
                cells[i, j] = c.GetComponent<FieldCell>();
                cells[i, j].CheckIfPossibleToStandOn
                    (
                        layerMask, 
                        fieldInfo.ImpossibleMoveTag,
                        fieldInfo.ImpossibleMoveCheckDistance
                    );
                cells[i, j].I = i;
                cells[i, j].J = j;
            }
        }
        fieldBuilt = true;
    }

    #region API
    public FieldCell NearestCell(Vector3 position)
    {
        if (!fieldBuilt)
            BuildField();
        //FieldCell res = cells[0, 0];
        //for (int i = 0; i < cells.GetLength(0); ++i)
        //    for (int j = 0; j < cells.GetLength(1); ++j)
        //        if ((cells[i, j].transform.position - position).magnitude < (res.transform.position - position).magnitude)
        //            res = cells[i, j];
        //return res;

        int i = 1, j = 0;
        float distance = Vector3.Distance(cells[0, 0].transform.position, position);
        float newDistance;

        for (; i < cells.GetLength(0); ++i)
        {
            newDistance = Vector3.Distance(cells[i, j].transform.position, position);
            if (newDistance > distance)
            {
                --i;
                break;
            }
            distance = newDistance;
        }
        for (; j < cells.GetLength(1); ++j)
        {
            newDistance = Vector3.Distance(cells[i, j].transform.position, position);
            if (newDistance > distance)
            {
                --j;
                break;
            }
            distance = newDistance;
        }

        return cells[i, j];
    }
    public void DeactivateActiveCells()
    {
        foreach (FieldCell c in activeCells)
            c.Disactivate();
        activeCells.Clear();
    }
    public void CorrectPersonPosition(Person p)
    {
        FieldCell cell = NearestCell(p.transform.position);
        p.transform.position = new Vector3(
            cell.transform.position.x,
            p.transform.position.y,
            cell.transform.position.z);
        RegisterPersonPosition(p, cell);
    }
    public void CorrectItemsPositions()
    {
        foreach(ResourcePhysic res in GameObject.FindObjectsOfType<ResourcePhysic>())
        {
            FieldCell c = NearestCell(res.transform.position);
            Vector3 colliderCenter = res.GetComponent<Collider>().bounds.center;
            res.transform.position = new Vector3(
                c.transform.position.x + res.transform.position.x - colliderCenter.x,
                res.transform.position.y,
                c.transform.position.z + res.transform.position.z - colliderCenter.z);

        }
    }
    public void RegisterPersonPosition(Person p, FieldCell cell)
    {
        dispatcher.RegisterNewPersonPosition(p, cell);
    }
    public void ActivateNearestCells(PersonActionController person)
    {
        activeCells.Clear();
        float distance = person.radiusInCells * CellSize;

        foreach(FieldCell c in NearestCells(person, distance))
            if (c.IsPossibleToStandOn)
            {
                c.Activate();
                activeCells.Add(c);
            }
    }
    public List<FieldCell> NearestCells(PersonActionController person)
    {
        return NearestCells(person, person.radiusInCells * CellSize);
    }
    public List<FieldCell> NearestCells(Person person, float radius)
    {
        List<FieldCell> nearestCells = new List<FieldCell>();

            //if (c.IsPossibleToStandOn && !dispatcher.IsAnyPersonOnCell(c) &&
            //    !dispatcher.IsCellStakedOut(c) && person.DistanceToCell(c) <= radius + CellSize / 10.0f)
            //    nearestCells.Add(c);
        FieldCell centralCell = dispatcher.GetPersonsPosition(person);
        List<Point> offsets = dispatcher.OffsetMasks[person.radiusInCells * CellSize];
        foreach (Point p in offsets)
        {
            int i = centralCell.I + p.X;
            int j = centralCell.J + p.Y;
            //print("i: " + i + ", j: " + j);
            //print(cells.GetLength(0));
            //print(cells.GetLength(1));
            if (i >= 0 && j >= 0)
            {
                //print("greater than zero");
                //print(cells.GetLength(0) < i ? "i is Ok" : "i isn't Ok");
                if (i < cells.GetLength(0) && j < cells.GetLength(1))
                {
                    FieldCell cell = cells[centralCell.I + p.X, centralCell.J + p.Y];
                    if (cell.IsPossibleToStandOn && !dispatcher.IsAnyPersonOnCell(cell) &&
                        !dispatcher.IsCellStakedOut(cell) &&
                        person.DistanceToCell(cell) <= radius + CellSize / 10.0f)
                        nearestCells.Add(cell);
                }
            }
        }
        return nearestCells;
    }
    public FieldCell GetActiveCellInPosition(Vector3 position)
    {
        foreach (FieldCell c in activeCells)
        {
            if (position.x >= c.transform.position.x - CellSize / 2.0f &&
                  position.x <= c.transform.position.x + CellSize / 2.0f &&
                  position.z >= c.transform.position.z - CellSize / 2.0f &&
                  position.z <= c.transform.position.z + CellSize / 2.0f
              )
                return c;
        }
        return null;
    }

    public ZombieController GetEnemyOnCell(FieldCell cell)
    {
        return dispatcher.GetEnemyOnCell(cell);
    }
    public void RemovePerson(Person p)
    {
        dispatcher.RemovePerson(p);
    }
    public void StakeOutCell(FieldCell c)
    {
        dispatcher.StakeOutCell(c);
    }
    public void FreeStakedOutCells()
    {
        dispatcher.FreeStakedOutCells();
    }
    #endregion
}
