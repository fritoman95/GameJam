using UnityEngine;

[CreateAssetMenu(fileName = "BuildingParts", menuName = "Scriptable Objects/BuildingParts")]
public class BuildingPartsSO : ScriptableObject
{
    public string BuildingPartName;

    public int MaxHealthPoints;
    public int DamagePoints;

    public float AttackTime;

    public int PartCost;

    public int ColumnAttackRange;

    public GameObject BuildingGameObject;

    [SerializeField]
    public MeshFilter PartMeshFilter;

    public Texture2D UISprite;
    public float ResizeValue;
    public float HighlightObjectVeritcalOffset;
}