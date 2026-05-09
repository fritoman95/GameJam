using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class PartInBuildMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BuildingParts Part;

    RawImage _partTexture;

    Collider _collider;

    bool _canBeClicked;

    public void Initialize()
    {
        _partTexture = GetComponent<RawImage>();
        _collider = GetComponent<Collider>();

        _partTexture.texture = Part.BuildingStats.UISprite;
    }

    public void ToggleClickability(bool value)
    {
        _canBeClicked = value;
    }

    public void PartClicked()
    {
        CastleBuilderController.Instance.ClickPart(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogWarning($"Called in on pointer click");
        PartClicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.LogWarning($"Called in on pointer enter");
        CastleBuilderController.Instance.UpdateCurrentBuildPartsUI(this);
    }

    public void OnPointerExit(PointerEventData eventData)
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

        Debug.LogWarning($"Called in on pointer exit");
    }
}
