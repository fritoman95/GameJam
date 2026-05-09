using System;
using TMPro;
using UnityEngine;

[Serializable]
public class SubPanelController : BuildUIBaseScript
{
    [SerializeField]
    TextMeshProUGUI _partName;
    [SerializeField]
    TextMeshProUGUI _healthValue;
    [SerializeField]
    TextMeshProUGUI _damageValue;

    public override void Initialize()
    {
        if(BuildMenuUIController.Instance.CurrentlySelectedPart)
        throw new System.NotImplementedException();
    }

    public void UpdateCurrentBuildPartsUI(PartInBuildMenu part)
    {
        if(part == null)
        {
            _partName.text = "";
            _healthValue.text =  "";
            _damageValue.text = "";

            return;
        }

        _partName.text = part.Part.BuildingStats.BuildingPartName;
        _healthValue.text = part.Part.BuildingStats.MaxHealthPoints.ToString();
        _damageValue.text = part.Part.BuildingStats.DamagePoints.ToString();
    }
}