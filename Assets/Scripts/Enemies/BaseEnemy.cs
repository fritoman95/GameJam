using System;
using UnityEngine;

[Serializable]
public class BaseEnemy : MonoBehaviour, IHealthSystem
{
    [SerializeField]
    EnemyStatsSO _stats;

    [SerializeField]
    BuildingParts _targettedBuildingPart;

    public int MaxHealth;
    public int CurrentHealth;
    public int MaxSpeed;
    public int CurrentSpeed;
    public int Damage;

    public int AttackRange;
    public int AttackSpeed;

    float _currentAttackTimer;

    void Awake()
    {
        Intialize();
    }

    public void Intialize()
    {
        SetHealthValues();
        
        MaxSpeed = _stats.Speed;
        CurrentSpeed = MaxSpeed;

        Damage = _stats.Damage;

        AttackRange = _stats.AttackRange;
        AttackSpeed = _stats.AttackSpeed;
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _targettedBuildingPart.transform.position);

        //have the enemymove down the lane
        if (distanceToTarget < AttackRange)
            MoveDownLane();
        //when they get in range to attack, attack the enemy
        else
            AttackEnemy();
    }

    public void MoveDownLane()
    {
        transform.position += Vector3.right * CurrentSpeed * Time.deltaTime;

        _currentAttackTimer = 0;
    }

    public void AttackEnemy()
    {
        _currentAttackTimer += Time.deltaTime;

        if(_currentAttackTimer >= AttackSpeed)
        {
            _currentAttackTimer = 0;
            _targettedBuildingPart.OnHealthChangeEvent(-Damage);
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
    public void OnHealthChangeEvent(int difference)
    {
        CurrentHealth += difference;

        if (CurrentHealth <= 0)
            OnDieEvent();
    }

    public void OnDieEvent()
    {
        //Send object back to pool

        //Stop movement or attacking if doing that

        //reward money
    }


}
