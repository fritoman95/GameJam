using System;
using System.Collections.Generic;
using UnityEngine;

public class CastleBuilderController : MonoBehaviour
{
    public static CastleBuilderController Instance;

    [SerializeField]
    PartInBuildMenu _currentlySelectedPart;
    public PartInBuildMenu CurrentlySelectedPart
    {
        get { return _currentlySelectedPart; }
        set { _currentlySelectedPart = value; }
    }

    [SerializeField]
    List<PartInBuildMenu> _listOfParts;

    public List<PartInBuildMenu> ListOfParts 
    { 
        get { return _listOfParts; }
        private set { _listOfParts = value; }
    }

    [Header("Highlight Objects Parameters")]
    [SerializeField]
    LayerMask _hittableLayers;

    [SerializeField]
    BuildPieceHighlight _buildPieceHighlight;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        _listOfParts.ForEach(x => x.Initialize());
    }

    void Update()
    {
        if (GameManager.CurrentState != GameState.Building || _currentlySelectedPart == null)
            return;

        //Move the part to be over where the players mouse is
        MoveBuildPreviewObject();
    }

    void MoveBuildPreviewObject()
    {
        if(GridManager.CurrentlyHoveredOverCell != null && !GridManager.CurrentlyHoveredOverCell.NonBuildableSpot)
        {
            _buildPieceHighlight.UpdatePiecesPosition(GridManager.CurrentlyHoveredOverCell.WorldPosition);

            bool showValidMaterial = !GridManager.CurrentlyHoveredOverCell.IsOccupied &&
                GameManager.Instance.Economy.CurrentMoneyValue >= _currentlySelectedPart.Part.BuildingStats.PartCost;

            _buildPieceHighlight.AssignCorrectHighlightMaterial(showValidMaterial);
            return;
        }

        if (Physics.Raycast(PlayerInputController.PlayersMouseRay, out RaycastHit hit, Mathf.Infinity, _hittableLayers))
        {
            _buildPieceHighlight.UpdatePiecesPosition(hit.point);
            _buildPieceHighlight.AssignCorrectHighlightMaterial(false);
        }
    }

    public void AssignCurrentSelectedPart(PartInBuildMenu part)
    {
        _currentlySelectedPart = part;
        _buildPieceHighlight.AssignCurrentSelectedPart(_currentlySelectedPart);
    }

    public void SpawnBuildingPiece()
    {
        if (!CanSpawnBuilding())
            return;

        GridCell cell = GridManager.CurrentlyHoveredOverCell;

        BuildingParts buildingPart = Instantiate(_currentlySelectedPart.Part, cell.WorldPosition, Quaternion.identity);

        buildingPart.InitializePart(cell);
        GridManager.Instance.SaveGridCellCombo(cell, buildingPart);

        GameManager.Instance.Economy.ChargeForPart(buildingPart.BuildingStats.PartCost);
    }

    bool CanSpawnBuilding()
    {
        if(GameManager.CurrentState != GameState.Building)
        {
            Debug.LogWarning($"Cannot Build, Not in build stateS");
            return false;
        }
        if (_currentlySelectedPart == null)
        {
            Debug.LogWarning($"Cannot Build, No selected part");
            return false;
        }
        if (GridManager.CurrentlyHoveredOverCell == null)
        {
            Debug.LogWarning($"Cannot Build, Not in a valid Cell");
            return false;
        }
        else if (GridManager.CurrentlyHoveredOverCell.IsOccupied)
        {
            Debug.LogWarning($"Cannot Build, Cell is occupieds");
            return false;
        }
        else if (GridManager.CurrentlyHoveredOverCell.NonBuildableSpot)
        {
            Debug.LogWarning($"Cannot Build, Cell is notBuildable");
            return false;
        }
        else if (GameManager.Instance.Economy.CurrentMoneyValue < _currentlySelectedPart.Part.BuildingStats.PartCost)
        {
            Debug.LogWarning($"Cannot Build, not enough money");
            return false;
        }

        return true;
    }
}

public class BuildingParts : MonoBehaviour
{
    public GridCell BuildingPartsBuildCell;

    public BuildingPartsSO BuildingStats;

    public float CurrentHealth;
    public float CurrentDamage;

    public void InitializePart(GridCell partsCell)
    {
        BuildingPartsBuildCell = partsCell;

        CurrentHealth = BuildingStats.MaxHealthPoints;
        CurrentDamage = BuildingStats.DamagePoints;
    }

    public void DestroyBuildPart()
    {
        GridManager.Instance.RemoveGridCellPair(BuildingPartsBuildCell);
    }
}

[Serializable]
public class BuildPieceHighlight
{
    [SerializeField]
    GameObject _buildHighlightObject;
    [SerializeField]
    MeshFilter _buildHighlightMeshFilter;
    [SerializeField]
    Renderer _buildHighlightRenderer;

    [SerializeField]
    Material _validBuildSpotMaterial;
    [SerializeField]
    Material _invalidBuildSpotMaterial;

    internal void AssignCurrentSelectedPart(PartInBuildMenu part)
    {
        if(part != null)
            _buildHighlightMeshFilter = part.Part.BuildingStats.PartMeshFilter;

        _buildHighlightRenderer.enabled = part != null;
    }

    internal void AssignCorrectHighlightMaterial(bool value)
    {
        _buildHighlightRenderer.material = value ? _validBuildSpotMaterial : _invalidBuildSpotMaterial;
    }

    internal void UpdatePiecesPosition(Vector3 position)
    {
        _buildHighlightObject.transform.position = position;
    }
}