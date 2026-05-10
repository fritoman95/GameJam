using System;

[Serializable]
public class EconomyController
{
    public int CurrentMoneyValue;

    public int StartingAmount = 100;

    public void Initialize()
    {
        CurrentMoneyValue = StartingAmount;
    }

    internal void ChargeForPart(int value)
    {
        CurrentMoneyValue -= value;
        EconomyUI.Instance.UpdateMoneyAmount();
    }

    internal void RewardMoney(int value)
    {
        CurrentMoneyValue += value;
        EconomyUI.Instance.UpdateMoneyAmount();
    }
}