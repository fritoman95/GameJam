using UnityEngine;

[CreateAssetMenu(fileName = "BuildingParts", menuName = "Scriptable Objects/BuildingParts")]
public class BuildingPartsSO : ScriptableObject
{
    public string BuildingPartName;

    public float MaxHealthPoints;
    public float DamagePoints;

    public float AttackTime;

    public int PartCost;

    public GameObject BuildingGameObject;

    [SerializeField]
    public MeshFilter PartMeshFilter;

    public Texture2D UISprite;
}