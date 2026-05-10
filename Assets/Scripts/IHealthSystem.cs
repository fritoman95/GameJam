using UnityEngine;

public interface IHealthSystem
{
    public void SetHealthValues();

    public void OnDieEvent();

    public bool OnHealthChangeEvent(int difference);
}