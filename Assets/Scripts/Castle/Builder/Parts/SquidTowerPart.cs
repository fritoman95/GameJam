using UnityEngine;

public class SquidTowerPart : BuildingParts
{
    [SerializeField]
    LayerMask _hittableLayers;

    float _currentAttackTimer;
    float _attackTimer;

    public override void InitializePart(GridCell partsCell)
    {
        _attackTimer = BuildingStats.AttackTime;
        //get the spots that the tower should be able to attak in
        base.InitializePart(partsCell);
    }

    void Update()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 5 / 2, Vector3.left, Quaternion.identity, GridManager.Instance.CellSize * NumberOfCellsInFrontThatAreHittable, _hittableLayers);

        if (hits.Length > 0)
            AttackTimer();
        else
            _currentAttackTimer = 0;
    }

    void AttackTimer()
    {
        if (_currentAttackTimer <= _attackTimer)
            _currentAttackTimer += Time.deltaTime;
        else
        {
            _currentAttackTimer = 0;
            Attack();
        }
    }

    void Attack()
    {

    }
}