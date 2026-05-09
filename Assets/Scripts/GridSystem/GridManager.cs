using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    //Was CellWidth
    public int Rows;
    //Was CellHeight
    public int Columns;

    [SerializeField]
    float CellSize = 5;

    [SerializeField]
    GridCell[,] _grid;

    [SerializeField]
    Dictionary<GridCell, BuildingParts> GridAndBuildingPieceDictionary;

    [SerializeField]
    List<GridCell> cellsList = new List<GridCell>();
    [SerializeField]
    List<BuildingParts> BuildingParts = new List<BuildingParts>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        CreateGrid();

        GridAndBuildingPieceDictionary = new Dictionary<GridCell, BuildingParts>();
    }

    void Update()
    {
        //Temp code
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (GetXY(hit.point, out int x, out int y))
            {
                GridCell cell = GetCell(x, y);

                if (!cell.IsOccupied)
                {
                    Instantiate(GameObject.CreatePrimitive(PrimitiveType.Capsule), cell.WorldPosition, Quaternion.identity);

                    cell.IsOccupied = true;

                    //Temporary
                    WallPart newWall = new WallPart();

                    GridAndBuildingPieceDictionary.Add(cell, newWall);

                    cellsList.Add(cell);
                    BuildingParts.Add(newWall);
                }
            }
        }
    }

    void CreateGrid()
    {
        _grid = new GridCell[Columns, Rows];

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y) + (Vector3.one * (CellSize / 2));

                _grid[x, y] = new GridCell(x, y, worldPos);
            }
        }
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position + new Vector3(x * CellSize, 0, y * CellSize);
    }

    public GridCell GetCell(int x, int y)
    {
        return _grid[x, y];
    }

    public bool GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 offset = worldPosition - transform.position;

        x = Mathf.FloorToInt(offset.x / CellSize);
        y = Mathf.FloorToInt(offset.z / CellSize);

        return x >= 0 && y >= 0 && x < Columns && y < Rows;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                Vector3 pos = GetWorldPosition(x, y) + (Vector3.one * (CellSize / 2));

                Gizmos.DrawWireCube(pos, Vector3.one * CellSize);
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 1000);
    }
}

[Serializable]
public class GridCell
{
    internal int X;
    internal int Y;
    
    internal Vector3 WorldPosition;
    
    internal bool IsOccupied;

    internal GridCell(int x, int y, Vector3 worldPosition)
    {
        X = x;
        Y = y;
        WorldPosition = worldPosition;
    }
}