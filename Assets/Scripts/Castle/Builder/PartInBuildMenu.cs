using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class PartInBuildMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BuildingParts Part;

    RawImage _partTexture;

    [SerializeField]
    bool _canBeClicked;

    public void Initialize()
    {
        _partTexture = GetComponent<RawImage>();

        _partTexture.texture = Part.BuildingStats.UISprite;
    }

    public void ToggleClickability(bool value)
    {
        _canBeClicked = value;
    }

    public void PartClicked()
    {
        if(_canBeClicked)
            CastleBuilderController.Instance.AssignCurrentSelectedPart(this);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (_canBeClicked)
            PartClicked();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        BuildMenuUIController.Instance.UpdateCurrentBuildPartsUI(this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if(CastleBuilderController.Instance.CurrentlySelectedPart == null)
        {
            //Clear current part tab
        }
        else if(CastleBuilderController.Instance.CurrentlySelectedPart != null
            && CastleBuilderController.Instance.CurrentlySelectedPart != this)
        {
            //Show the current part tab
        }
    }
}
