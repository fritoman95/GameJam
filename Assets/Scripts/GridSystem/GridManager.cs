using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public static GridCell CurrentlyHoveredOverCell;

    [SerializeField]
    GridCell[,] _grid;

    [Header("Grid Parameters")]
    //Was CellWidth
    public int Rows;
    //Was CellHeight
    public int Columns;

    [SerializeField]
    float CellSize = 5;

    [Header("NonBuilding Grid Parameters")]
    public int NonBuildingColumnLimits;

    [SerializeField]
    Dictionary<GridCell, BuildingParts> _gridAndBuildingPieceDictionary;

    [SerializeField]
    List<GridCell> _cells = new List<GridCell>();
    [SerializeField]
    List<BuildingParts> _buildingParts = new List<BuildingParts>();

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

        _gridAndBuildingPieceDictionary = new Dictionary<GridCell, BuildingParts>();
    }

    void Update()
    {
        //Temp code
        if (Physics.Raycast(PlayerInputController.PlayersMouseRay, out RaycastHit hit))
        {
            GridCell cell = null;
            if (GetXY(hit.point, out int x, out int y))
                cell = GetCell(x, y);

            CurrentlyHoveredOverCell = cell;
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

    public void SaveGridCellCombo(GridCell cell, BuildingParts part)
    {
        cell.IsOccupied = true;

        _gridAndBuildingPieceDictionary.Add(cell, part);

        _cells.Add(cell);
        _buildingParts.Add(part);
    }

    public void RemoveGridCellPair(GridCell cell = null)
    {
        if (cell == null)
            throw new System.Exception("Cannot remove a part if nothing is provided");

        _gridAndBuildingPieceDictionary.TryGetValue(cell, out BuildingParts value);

        _cells.Remove(cell);
        _buildingParts.Remove(value);
        _gridAndBuildingPieceDictionary.Remove(cell);
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
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                GridCell grabbedCell = GetCell(x, y);
                Vector3 pos = grabbedCell.WorldPosition/*GetWorldPosition(x, y)*/ + (Vector3.one * (CellSize / 2));

                if (grabbedCell.BuildableGridSpot)
                    Gizmos.color = Color.gray;
                if (grabbedCell.IsOccupied)
                    Gizmos.color = Color.red;

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
    public int Column { get; private set; }
    public int Row { get; private set; }

    internal Vector3 WorldPosition;
    
    internal bool IsOccupied;
    internal bool BuildableGridSpot;

    internal GridCell(int x, int y, Vector3 worldPosition)
    {
        Column = x;
        Row = y;
        WorldPosition = worldPosition;

        BuildableGridSpot = Column < GridManager.Instance.NonBuildingColumnLimits;
    }
}