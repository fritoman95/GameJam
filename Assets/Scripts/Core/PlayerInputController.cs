using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController Instance;

    public static Vector2 PlayersMousePosition01 => Camera.main.ScreenToViewportPoint(Input.mousePosition);
    public static Ray PlayersMouseRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CastleBuilderController.Instance.SpawnBuildingPiece();
        if (Input.GetMouseButtonDown(1))
            CastleBuilderController.Instance.AssignCurrentSelectedPart(null);
    }


    void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 1000);
    }
}
