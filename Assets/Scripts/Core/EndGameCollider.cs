using System;
using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
    public int GoalMaxHealth = 1;

    [SerializeField]
    int _currentGoalHealth;

    public Action OnGameEnd;

    void Awake()
    {
        _currentGoalHealth = GoalMaxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.CurrentState != GameState.Defending)
            return;

        if(other.TryGetComponent(out BaseEnemy enemy))
        {
            Debug.LogWarning($"{enemy.name} has class BaseEnemy, goal is taking damage");
            _currentGoalHealth--;
        }

        if (_currentGoalHealth <= 0)
            OnGameEnd.Invoke();
    }
}