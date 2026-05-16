using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public static GridCell CurrentlyHoveredOverCell;

    [SerializeField]
    GridCell[,] _grid;

    [SerializeField]
    List<GridCell> _cells = new List<GridCell>();

    [SerializeField]
    GameObject _buildableFloor;
    [SerializeField]
    List<Material> _buildableSpotMaterials;

    Vector3 _buildableFloorSize = new Vector3(5, .01f, 5);

    [Header("Grid Parameters")]
    //Was CellWidth
    public int Rows;
    //Was CellHeight
    public int Columns;

    public float CellSize = 5;

    [Header("NonBuilding Grid Parameters")]
    public int NonBuildingColumnLimits;

    [SerializeField]
    Dictionary<GridCell, BuildingParts> _gridAndBuildingPieceDictionary;

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

        int numberOfSpawns = 0;

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                numberOfSpawns++;

                Vector3 worldPos = GetWorldPosition(x, y);

                GridCell thisCell = new GridCell(x, y, worldPos);

                Vector3 spawnPoint = thisCell.WorldPosition;
                spawnPoint.y = 0;

                if(!thisCell.NonBuildableSpot)
                {
                    GameObject floor = Instantiate(_buildableFloor, spawnPoint, Quaternion.identity);
                    floor.GetComponent<Renderer>().material = _buildableSpotMaterials[numberOfSpawns % 2];
                    floor.transform.localScale = _buildableFloorSize;
                }

                _grid[x, y] = thisCell;
                _cells.Add(thisCell);
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
        return (transform.position + new Vector3(x * CellSize, 0, y * CellSize)) + (Vector3.one * (CellSize / 2));
    }

    public GridCell GetCell(int x, int y)
    {
#if UNITY_EDITOR
        if (_grid == null)
            return null;
#endif

        GridCell cell = _grid[x, y];

        if (cell != null)
            return cell;

        return null;
    }

    public bool GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 offset = worldPosition - transform.position;

        x = Mathf.FloorToInt(offset.x / CellSize);
        y = Mathf.FloorToInt(offset.z / CellSize);

        return x >= 0 && y >= 0 && x < Columns && y < Rows;
    }

    public BuildingParts GetFirstCellInRowWithTower(int row)
    {
        GridCell firstOccupiedCellInRow = _cells.Where(x => x.Row == row).ToList().FirstOrDefault(x => x.IsOccupied);

        if (firstOccupiedCellInRow != null)
            return _gridAndBuildingPieceDictionary.GetValueOrDefault(firstOccupiedCellInRow);
        else
            return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if(Application.isPlaying)
            DrawireGridCubes();
        else
            DrawireGridCubesOutOfPlayMode();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 1000);
    }

    void DrawireGridCubes()
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                GridCell grabbedCell = GetCell(x, y);

                if (grabbedCell == null)
                    continue;

                Vector3 pos = grabbedCell.WorldPosition;

                if (grabbedCell.NonBuildableSpot)
                    Gizmos.color = Color.gray;
                if (grabbedCell.IsOccupied)
                    Gizmos.color = Color.red;
                if (!grabbedCell.NonBuildableSpot && !grabbedCell.IsOccupied)
                    Gizmos.color = Color.green;

                Gizmos.DrawWireCube(pos, Vector3.one * CellSize);
            }
        }
    }
    void DrawireGridCubesOutOfPlayMode()
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                Vector3 pos = GetWorldPosition(x, y);

                if (x < NonBuildingColumnLimits)
                    Gizmos.color = Color.gray;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawWireCube(pos, Vector3.one * CellSize);
            }
        }
    }
}

[Serializable]
public class GridCell
{
    public int Column { get; private set; }
    public int Row { get; private set; }

    internal Vector3 WorldPosition;
    
    internal bool IsOccupied;
    internal bool NonBuildableSpot;

    internal GridCell(int x, int y, Vector3 worldPosition)
    {
        Column = x;
        Row = y;
        WorldPosition = worldPosition;

        NonBuildableSpot = Column < GridManager.Instance.NonBuildingColumnLimits;
    }
}