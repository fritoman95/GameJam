using UnityEngine;

public interface IHealthSystem
{
    public void SetHealthValues();

    public void OnDieEvent();

    public void OnHealthChangeEvent(int difference);
}