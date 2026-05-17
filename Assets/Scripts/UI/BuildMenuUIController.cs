using DG.Tweening;
using UnityEngine;

public class BuildMenuUIController : UIBase
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

    bool DetermineMouseInActivationThreshold => PlayerInputController.PlayersMousePosition01.y >= 0 && PlayerInputController.PlayersMousePosition01.y <= BuildMenuShowingActivationThreshold;
    bool DetermineMouseInHidingThreshold => PlayerInputController.PlayersMousePosition01.y >= BuildMenuHidingThreshold;

    [SerializeField]
    RectTransform _buildMenuMoveObject;

    [SerializeField]
    AnimationCurve _buildMenuMoveAnimationCurve;

    const float BuildMenuMoveTime = .35f;
    const float BuildWindowOutYValue = 200;

    Vector2 _buildWindowOriginPosition;

    Tween _windowMoveTween;

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        if (GameManager.CurrentState == GameState.NotPlaying || GameManager.CurrentState == GameState.GameOver)
            return;

        if (!_showingBuildMenu && GameManager.CurrentState != GameState.Defending)
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

    public void HideBuildMenu()
    {
        _currentHidingTimer = 0;
        _showingBuildMenu = false;

        Vector2 targetPosition = _buildWindowOriginPosition;
        targetPosition.y -= BuildWindowOutYValue;

        MoveBuildWindowsPosition(targetPosition);
        UpdateCurrentBuildPartsUI(null);
        ToggleBuildingUIInteractivity(false);
    }

    void MoveBuildWindowsPosition(Vector2 position, bool useTime = true)
    {
        if (_windowMoveTween != null || _windowMoveTween.IsActive())
            _windowMoveTween.Kill(false);

        float moveTime = useTime ? BuildMenuMoveTime : 0;

        _windowMoveTween = _buildMenuMoveObject.DOAnchorPos(position, moveTime).SetEase(_buildMenuMoveAnimationCurve);
    }

    public void UpdateCurrentBuildPartsUI(PartInBuildMenu part)
    {
        _subPanelController.UpdateCurrentBuildPartsUI(part);
    }

    public override void EnablePanel()
    {
        //initialize everything the UI will need here
        _subPanelController.Initialize();

        _buildWindowOriginPosition = _buildMenuMoveObject.anchoredPosition;

        Vector2 targetPosition = _buildWindowOriginPosition;
        targetPosition.y -= BuildWindowOutYValue;

        MoveBuildWindowsPosition(targetPosition, useTime: false);

        _uiParentObject.SetActive(true);
    }

    public override void DisablePanel()
    {
        _uiParentObject.SetActive(false);
    }
}