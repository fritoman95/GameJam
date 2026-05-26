using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquidTowerPart : BuildingPart
{
    [SerializeField]
    Animator _squidTowerAnimator;

    [SerializeField]
    GameObject _inkBall;

    [SerializeField]
    Transform _firePosition;

    [SerializeField]
    LayerMask _hittableLayers;

    [SerializeField]
    float _currentAttackTimer;
    [SerializeField]
    float _attackTimer;

    [SerializeField]
    float _inkBallTravelTime = .45f;

    RaycastHit[] _hitTargets;

    [SerializeField]
    BaseEnemy _closestEnemy;

    [SerializeField]
    List<GridCell> _cellsWithinRange = new List<GridCell>();
    [SerializeField]
    List<BaseEnemy> _enemiesWithinAttackRange = new List<BaseEnemy>();

    public override void InitializePart(GridCell partsCell)
    {
        _attackTimer = BuildingStats.AttackTime;

        //get the spots that the tower should be able to attak in
        base.InitializePart(partsCell);

        CellsInRangeSubscriptions();
    }

    void CellsInRangeSubscriptions()
    {
        _cellsWithinRange = GridManager.Instance.GridCells.Where(x => x.Row == OccupyingCell.Row)
                                                                        .Where(x => x.Column >= OccupyingCell.Column - BuildingStats.ColumnAttackRange && x.Column < OccupyingCell.Column).ToList();

        for (int i = 0; i < _cellsWithinRange.Count; i++)
        {
            _cellsWithinRange[i].OnEnemiesAdded += AddEnemyWithinRange;
            _cellsWithinRange[i].OnEnemiesRemoved += RemoveEnemyWithinRange;
        }

        AssignEnemiesWithinAttackRange();
    }

    //This method should grab a list of all the cells within the towers attack range
    void AssignEnemiesWithinAttackRange()
    {   
        _cellsWithinRange.ForEach(x => AddEnemiesWithinRange(x.EnemiesOnCell));
    }

    void AddEnemyWithinRange(BaseEnemy enemy)
    {
        if(_enemiesWithinAttackRange.Contains(enemy))
        {
            Debug.LogWarning($"Tower already has enemy: {enemy} within range");
            return;
        }

        Debug.LogWarning($"adding enemy: {enemy} to range");
        _enemiesWithinAttackRange.Add(enemy);

        if (_enemiesWithinAttackRange.Count == 1)
            _closestEnemy = enemy;
    }
    void AddEnemiesWithinRange(List<BaseEnemy> enemies)
    {
        foreach (BaseEnemy enemy in enemies)
        {
            AddEnemyWithinRange(enemy);
        }
    }
    void RemoveEnemyWithinRange(BaseEnemy enemy)
    {
        if (!_enemiesWithinAttackRange.Contains(enemy))
        {
            Debug.LogWarning($"Tower does not have enemy: {enemy} within range");
            return;
        }

        Debug.LogWarning($"removing enemy: {enemy} from range");
        _enemiesWithinAttackRange.Remove(enemy);

        if (_closestEnemy == enemy)
            _closestEnemy = null;
    }
    void RemoveEnemiesWithinRange(List<BaseEnemy> enemies)
    {
        foreach (BaseEnemy enemy in enemies)
        {
            RemoveEnemyWithinRange(enemy);
        }
    }

    void Update()
    {
        if (_enemiesWithinAttackRange.Count == 0)
            return;

        if(_enemiesWithinAttackRange.Count > 1)
            _closestEnemy = AssignClosestEnemy();

        //Old attack code
        //_hitTargets = Physics.BoxCastAll(transform.position, Vector3.one * 5 / 2, Vector3.left, Quaternion.identity, GridManager.Instance.CellSize * NumberOfCellsInFrontThatAreHittable, _hittableLayers, QueryTriggerInteraction.Collide);

        if (_closestEnemy != null)
            AttackTimer();
        else
            _currentAttackTimer = 0;
    }

    BaseEnemy AssignClosestEnemy()
    {
        for (int i = 0; i < _enemiesWithinAttackRange.Count; i++)
        {
            BaseEnemy enemy = _enemiesWithinAttackRange[i];

            if (enemy == null)
                continue;

            if (_closestEnemy == null || (enemy != _closestEnemy && enemy.transform.position.x > _closestEnemy.transform.position.x))
                return enemy;
        }

        Debug.LogError("Could Not return closest enemy");

        return null;
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
        //Play attack animation here

        Sequence attackSequence = DOTween.Sequence();

        attackSequence.AppendCallback(() =>
        {
            _squidTowerAnimator.SetTrigger("Attack");
        });
        attackSequence.AppendInterval(.45f).AppendCallback(() =>
        {
            GameObject inkBall = Instantiate(_inkBall, _firePosition.position, Quaternion.identity);

            inkBall.transform.DOMove(_hitTargets[0].collider.transform.position, _inkBallTravelTime).OnComplete(() =>
            {
                _hitTargets[0].collider.GetComponent<BaseEnemy>().OnHealthChangeEvent(-CurrentDamage);
                Destroy(inkBall);
            });
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(transform.position, Vector3.left * GridManager.Instance.CellSize * NumberOfCellsInFrontThatAreHittable);
    }
}