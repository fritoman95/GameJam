using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    public int Speed;

    public int MaxHealth;

    public int Damage;

    public float AttackRange;
    public int AttackSpeed;
}