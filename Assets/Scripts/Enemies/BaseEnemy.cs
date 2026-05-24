using System;
using UnityEngine;

[Serializable]
public class BaseEnemy : MonoBehaviour, IHealthSystem, IConstantGridCellUpdater
{
    public int Row;
    public GridCell CellCurrentlyOn;

    [SerializeField]
    EnemyStatsSO _stats;

    [SerializeField]
    BuildingPart _targettedBuildingPart;

    public int MaxHealth;
    public int CurrentHealth;
    public int MaxSpeed;
    public int CurrentSpeed;
    public int Damage;

    public float AttackRange;
    public int AttackSpeed;

    float _currentAttackTimer;

    bool _isWalking;

    [SerializeField]
    Animator _enemyAnimator;

    public void Intialize(int row, int column = 0)
    {
        Row = row;

        CellCurrentlyOn = GridManager.Instance.GetCell(column, row);

        SetHealthValues();
        
        MaxSpeed = _stats.Speed;
        CurrentSpeed = MaxSpeed;

        Damage = _stats.Damage;

        AttackRange = _stats.AttackRange;
        AttackSpeed = _stats.AttackSpeed;

        _targettedBuildingPart = GridManager.Instance.GetFirstCellInRowWithTower(Row);
    }

    void Update()
    {
        if(_targettedBuildingPart == null)
            _targettedBuildingPart = GridManager.Instance.GetFirstCellInRowWithTower(Row);

        if (_targettedBuildingPart != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targettedBuildingPart.transform.position);

            //have the enemymove down the lane
            if (distanceToTarget > AttackRange)
                MoveAcrossLane();
            //when they get in range to attack, attack the enemy
            else
                AttackEnemy();
        }
        else
            MoveAcrossLane();

        _enemyAnimator.SetBool("Walking", _isWalking);

        UpdateGridCell();
    }

    public void MoveAcrossLane()
    {
        transform.position += Vector3.right * CurrentSpeed * Time.deltaTime;

        _isWalking = true;

        _currentAttackTimer = 0;
    }

    public void AttackEnemy()
    {
        _isWalking = false;
        _currentAttackTimer += Time.deltaTime;

        if(_currentAttackTimer >= AttackSpeed)
        {
            _currentAttackTimer = 0;

            if (_targettedBuildingPart.OnHealthChangeEvent(-Damage))
                _targettedBuildingPart = GridManager.Instance.GetFirstCellInRowWithTower(Row);

            _enemyAnimator.SetTrigger("Attack");
        }
    }

    public void SetHealthValues()
    {
        MaxHealth = _stats.MaxHealth;
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// This will be called whenever an enemy gets their health changed
    /// negative numbers is damage, positive is healing
    /// </summary>
    /// <param name="difference"></param>
    public bool OnHealthChangeEvent(int difference)
    {
        CurrentHealth += difference;

        if (CurrentHealth <= 0)
            OnDieEvent();

        return CurrentHealth <= 0;
    }

    public void OnDieEvent()
    {
        //Stop movement or attacking if doing that
        Destroy(gameObject);

        //reward money
        GameManager.Instance.Economy.RewardMoney(_stats.KillReward);
    }

    public void UpdateGridCell()
    {
        GridCell checkedCell = GridManager.Instance.GetCell(transform.position);

        if (checkedCell == null || CellCurrentlyOn == checkedCell)
            return;
        else
        {
            GridManager.Instance.UpdateCellAndEnemyInformation(checkedCell, this);

            CellCurrentlyOn = checkedCell;
        }
    }
}