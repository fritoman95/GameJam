using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUIController : UIBase
{
    public static BuildMenuUIController Instance;
    public PartInBuildMenu CurrentlySelectedPart => CastleBuilderController.Instance.CurrentlySelectedPart;

    [SerializeField]
    SubPanelController _subPanelController;

    [SerializeField]
    Button _startWaveButton;

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
    RectTransform _startbuttonTransform;

    [SerializeField]
    AnimationCurve _buildMenuMoveAnimationCurve;
    [SerializeField]
    AnimationCurve _startButtonAnimationCurve;

    const float BuildMenuMoveTime = .35f;
    const float BuildWindowOutYValue = 200;
    const int _startButtonHiddenYOffset = 100;
    const float StartButtonMoveTime = .35f;

    Vector2 _buildWindowOriginPosition;
    Vector2 _startButtonShowingPosition;

    Tween _windowMoveTween;
    Tween _sendWaveMoveTween;

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        ToggleSendWaveButtonVisibility(false, true);

        _startButtonShowingPosition = _startbuttonTransform.anchoredPosition;
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

    public void SendWaveButtonPressed()
    {
        Debug.LogWarning("1");
        ToggleSendWaveButton(false);
        Debug.LogWarning("2");
        ToggleSendWaveButtonVisibility(false);
        Debug.LogWarning("3");
        HideBuildMenu();
        Debug.LogWarning("4");

        CastleBuilderController.Instance.AssignCurrentSelectedPart(null);
        Debug.LogWarning("5");
        GameManager.Instance.ChangeState(GameState.Defending);
        Debug.LogWarning("6");
    }

    void ToggleSendWaveButton(bool value)
    {
        //drop in money tab
        if (value)
            ToggleSendWaveButtonVisibility(true);

        _startWaveButton.enabled = value;
    }

    public void ToggleSendWaveButtonVisibility(bool visible, bool snap = false)
    {
        if (_sendWaveMoveTween != null || _sendWaveMoveTween.IsActive())
            _sendWaveMoveTween.Kill(false);

        Vector2 newPosition = _startButtonShowingPosition;
        if (!visible)
            newPosition.y += _startButtonHiddenYOffset;

        float newTime = !snap ? StartButtonMoveTime : 0;

        _sendWaveMoveTween = _startbuttonTransform.DOAnchorPos(newPosition, newTime).SetEase(_startButtonAnimationCurve);
    }

    public override void EnablePanel()
    {
        //initialize everything the UI will need here
        _subPanelController.Initialize();
        //Set the current money amount
        ToggleSendWaveButton(true);

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