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
    List<GridCell> _gridCells = new List<GridCell>();
    HashSet<GridCell> _gridCellsHashSet = new HashSet<GridCell>();

    [Header("Grid Parameters")]
    //Was CellWidth
    public int Rows;
    //Was CellHeight
    public int Columns;

    public float CellSize = 5;

    [Header("NonBuilding Grid Parameters")]
    public int NonBuildingColumnLimits;

    [SerializeField]
    Dictionary<GridCell, BuildingPart> _gridAndBuildingPieceDictionary;

    List<GridCell> _currentlyOccupiedCells => _currentCellsWithEnemies.Concat(_currentCellsWithTowers).Distinct().ToList();
    List<GridCell> _currentCellsWithEnemies => _gridCells.Where(x => x.EnemiesOnCell.Count > 0).ToList();
    List<GridCell> _currentCellsWithTowers => _gridCells.Where(x => x.BuildingPartsOnCell.Count > 0).ToList();

    [SerializeField]
    List<GridCell> TEMP_currentlyOccupiedCells;
    [SerializeField]
    List<GridCell> TEMP_currentCellsWithEnemies;
    [SerializeField]
    List<GridCell> TEMP_currentCellsWithTowers;

    [Header("Possibly temporary ground")]
    [SerializeField]
    GameObject _buildableFloor;
    [SerializeField]
    List<Material> _buildableSpotMaterials;

    Vector3 _buildableFloorSize = new Vector3(5, .01f, 5);

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

        _gridAndBuildingPieceDictionary = new Dictionary<GridCell, BuildingPart>();
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

        TEMP_currentlyOccupiedCells = _currentlyOccupiedCells;
        TEMP_currentCellsWithEnemies = _currentCellsWithEnemies;
        TEMP_currentCellsWithTowers = _currentCellsWithTowers;
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
                _gridCells.Add(thisCell);
            }
        }
    }

    public void SaveCellAndBuildingPart(GridCell cell, BuildingPart part)
    {
        _gridAndBuildingPieceDictionary.Add(cell, part);
        cell.BuildingPartsOnCell.Add(part);
    }

    public void UpdateCellAndEnemyInformation(GridCell newCell, BaseEnemy enemy)
    {
        //If the enemy is currently on a different cell, unassign it
        if(enemy.CellCurrentlyOn != null)
            enemy.CellCurrentlyOn.EnemiesOnCell.Remove(enemy);

        newCell.EnemiesOnCell.Add(enemy);
    }

    public void RemoveGridCellPair(GridCell cell = null)
    {
        if (cell == null)
            throw new Exception("Cannot remove a part if nothing is provided");

        _gridAndBuildingPieceDictionary.TryGetValue(cell, out BuildingPart value);

        if (value == null)
            throw new Exception($"No Building Part was grabbed at cell {cell}, could not remove");

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

    public GridCell GetCell(Vector3 worldPosition)
    {
        if (GetXY(worldPosition, out int x, out int y))
            return GetCell(x, y);
        else
            return null;
    }

    public bool GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 offset = worldPosition - transform.position;

        x = Mathf.FloorToInt(offset.x / CellSize);
        y = Mathf.FloorToInt(offset.z / CellSize);

        return x >= 0 && y >= 0 && x < Columns && y < Rows;
    }

    public BuildingPart GetFirstCellInRowWithTower(int row)
    {
        GridCell firstOccupiedCellInRow = _gridCells.Where(x => x.Row == row).ToList().FirstOrDefault(x => x.IsOccupied);

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
    [SerializeField]
    int _column;
    public int Column => _column;
    [SerializeField]
    int _row;
    public int Row => _row;

    internal Vector3 WorldPosition;

    public List<BaseEnemy> EnemiesOnCell = new List<BaseEnemy>();
    public List<BuildingPart> BuildingPartsOnCell = new List<BuildingPart>();
    
    internal bool IsOccupied => BuildingPartsOnCell.Count > 0;
    internal bool NonBuildableSpot;

    internal GridCell(int x, int y, Vector3 worldPosition)
    {
        _column = x;
        _row = y;
        WorldPosition = worldPosition;

        NonBuildableSpot = Column < GridManager.Instance.NonBuildingColumnLimits;
    }
}

interface IGridCellUpdater
{
    public void UpdateGridCell();
}

interface IConstantGridCellUpdater
{
    public void UpdateGridCell();
}