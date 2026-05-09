using DG.Tweening;
using TMPro;
using UnityEngine;

public class BuildMenuUIController : MonoBehaviour
{
    public static BuildMenuUIController Instance;
    public PartInBuildMenu CurrentlySelectedPart => CastleBuilderController.Instance.CurrentlySelectedPart;

    [SerializeField]
    SubPanelController _subPanelController;

    [SerializeField]
    bool _showingBuildMenu;

    const float BuildMenuShowingActivationThreshold = .1f;
    const float BuildMenuHidingThreshold = .5f;

    [SerializeField]
    float _currentHidingTimer;
    [SerializeField]
    float _hidingTimerThreshold;

    bool DetermineMouseInActivationThreshold => Camera.main.ScreenToViewportPoint(Input.mousePosition).y <= BuildMenuShowingActivationThreshold;
    bool DetermineMouseInHidingThreshold => Camera.main.ScreenToViewportPoint(Input.mousePosition).y >= BuildMenuHidingThreshold;

    [SerializeField]
    RectTransform _buildMenuMoveObject;

    [SerializeField]
    AnimationCurve _buildMenuMoveAnimationCurve;

    float _buildMenuMoveTime = .35f;

    [SerializeField]
    float _buildWindowInYValue = 0;
    [SerializeField]
    float _buildWindowOutYValue = 200;

    Vector2 _buildWindowOriginPosition;

    Tween _windowMoveTween;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        //initialize everything the UI will need here
        _subPanelController.Initialize();

        _buildWindowOriginPosition = _buildMenuMoveObject.anchoredPosition;

        Vector2 targetPosition = _buildWindowOriginPosition;
        targetPosition.y -= _buildWindowOutYValue;

        MoveBuildWindowsPosition(targetPosition, useTime: false);
    }

    void Update()
    {
        if (!_showingBuildMenu)
        {
            if(DetermineMouseInActivationThreshold)
                ShowBuildMenu();
        }
        else
        {
            _currentHidingTimer = DetermineMouseInHidingThreshold ? _currentHidingTimer + Time.deltaTime : 0;

            if (_currentHidingTimer >= _hidingTimerThreshold)
                HideBuildMenu();
        }
    }

    void ToggleBuildingUIInteractivity(bool value)
    {
        //Turn on the window and allow the player to grab the parts
        CastleBuilderController.Instance.ListOfParts.ForEach(x => x.ToggleClickability(value));
    }

    void ShowBuildMenu()
    {
        _showingBuildMenu = true;

        MoveBuildWindowsPosition(_buildWindowOriginPosition);
        ToggleBuildingUIInteractivity(true);
    }

    void HideBuildMenu()
    {
        _currentHidingTimer = 0;
        _showingBuildMenu = false;

        Vector2 targetPosition = _buildWindowOriginPosition;
        targetPosition.y -= _buildWindowOutYValue;

        MoveBuildWindowsPosition(targetPosition);
        UpdateCurrentBuildPartsUI(null);
        ToggleBuildingUIInteractivity(false);
    }

    void MoveBuildWindowsPosition(Vector2 position, bool useTime = true)
    {
        if (_windowMoveTween != null || _windowMoveTween.IsActive())
            _windowMoveTween.Kill(false);

        float moveTime = useTime ? _buildMenuMoveTime : 0;

        _windowMoveTween = _buildMenuMoveObject.DOAnchorPos(position, moveTime).SetEase(_buildMenuMoveAnimationCurve);
    }

    public void UpdateCurrentBuildPartsUI(PartInBuildMenu part)
    {
        _subPanelController.UpdateCurrentBuildPartsUI(part);
    }
}