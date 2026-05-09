using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CastleBuilderController : MonoBehaviour
{
    public static CastleBuilderController Instance;

    [SerializeField]
    List<PartInBuildMenu> _listOfParts;

    [SerializeField]
    Material _validBuildSpotMaterial;
    [SerializeField]
    Material _invalidBuildSpotMaterial;

    [Header("Highlight Objects Parameters")]
    [SerializeField]
    LayerMask _hittableLayers;

    [SerializeField]
    GameObject _buildHighlightObject;
    [SerializeField]
    MeshFilter _buildHighlightMeshFilter;

    [SerializeField]
    TextMeshProUGUI _partName;
    [SerializeField]
    TextMeshProUGUI _healthValue;
    [SerializeField]
    TextMeshProUGUI _damageValue;

    PartInBuildMenu _currentlySelectedPart;
    public PartInBuildMenu CurrentlySelectedPart => _currentlySelectedPart;

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
        if(_currentlySelectedPart != null)
        {
            //Move the part to be over where the players mouse is
            MoveBuildPreviewObject();

            //Assign the correct material based off whether the position it is over is a valid build spot
            AssignBuildHighlightMaterial();

            if(Input.GetMouseButtonDown(0))
                SpawnBuildingPiece(_currentlySelectedPart);
        }
    }

    void MoveBuildPreviewObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _hittableLayers))
        {
            _buildHighlightObject.transform.position = hit.point;

            Debug.Log("Hit: " + hit.transform.name);
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }

        //Make a raycast that will shoot towards the ground

        //Move the highlightObject to the position;
    }

    void AssignBuildHighlightMaterial()
    {
        _buildHighlightObject.GetComponent<Renderer>().material = _validBuildSpotMaterial;
    }

    public void ShowPartSubWindow()
    {
        //assign subwindow assets

        //Pop up part subwindow
    }
    
    public void HidePartSubWindow()
    {
        //hide part subwindow
    }

    void EnableBuildingWindow()
    {
        //Turn on the window and allow the player to grab the parts
        _listOfParts.ForEach(x => x.ToggleClickability(true));
    }

    public void ClickPart(PartInBuildMenu part)
    {
        _currentlySelectedPart = part;

        _buildHighlightMeshFilter = _currentlySelectedPart.Part.BuildingStats.PartMeshFilter;


    }

    public void UpdateCurrentBuildPartsUI(PartInBuildMenu part)
    {
        _partName.text = part.Part.BuildingStats.name;
        _healthValue.text = part.Part.BuildingStats.MaxHealthPoints.ToString();
        _damageValue.text = part.Part.BuildingStats.DamagePoints.ToString();
    }

    void DropPart(PartInBuildMenu part)
    {
        //SpawnBuildingPiece
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

    void Start()
    {
    }

    public void InitializePart()
    {
        CurrentHealth = BuildingStats.MaxHealthPoints;
        CurrentDamage = BuildingStats.DamagePoints;
    }
}