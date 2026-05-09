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
        if (_currentlySelectedPart == null)
            return;

        //Move the part to be over where the players mouse is
        MoveBuildPreviewObject();

        //Assign the correct material based off whether the position it is over is a valid build spot
        _buildPieceHighlight.AssignBuildHighlightMaterial();

        if (Input.GetMouseButtonDown(0))
            SpawnBuildingPiece(_currentlySelectedPart);
        if (Input.GetMouseButtonDown(1))
            AssignCurrentSelectedPart(null);
    }

    void MoveBuildPreviewObject()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, _hittableLayers))
            _buildPieceHighlight.UpdatePiecesPosition(hit.point);
    }

    public void AssignCurrentSelectedPart(PartInBuildMenu part)
    {
        _currentlySelectedPart = part;
        _buildPieceHighlight.AssignCurrentSelectedPart(_currentlySelectedPart);
    }

    void SpawnBuildingPiece(PartInBuildMenu part)
    {
        if (_currentlySelectedPart == null)
            return;

        //BuildingParts newPart = GameObject.Instantiate(part);
    }
}

public class BuildingParts : MonoBehaviour
{
    public BuildingPartsSO BuildingStats;

    public float CurrentHealth;
    public float CurrentDamage;

    public void InitializePart()
    {
        CurrentHealth = BuildingStats.MaxHealthPoints;
        CurrentDamage = BuildingStats.DamagePoints;
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

    }

    internal void UpdatePiecesPosition(Vector3 position)
    {
        _buildHighlightObject.transform.position = position;
    }

    internal void AssignBuildHighlightMaterial()
    {
        _buildHighlightObject.GetComponent<Renderer>().material = _validBuildSpotMaterial;
    }
}