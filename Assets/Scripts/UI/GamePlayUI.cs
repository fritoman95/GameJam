using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI Instance;

    [SerializeField]
    Button _startWaveButton;

    [SerializeField]
    RectTransform _startbuttonTransform;

    Vector2 _startButtonShowingPosition;
    int _startButtonHiddenYOffset = 100;

    [SerializeField]
    AnimationCurve _startButtonAnimationCurve;
    const float StartButtonMoveTime = .35f;

    Tween _windowMoveTween;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        _startButtonShowingPosition = _startbuttonTransform.anchoredPosition;
    }

    void Start()
    {
        ToggleSelectButtonVisibility(false, true);
    }

    public void Initialize()
    {
        //Set the current money amount
        ToggleSendWaveButton(true);

        //drop in money tab
        ToggleSelectButtonVisibility(true);
    }

    void ToggleSendWaveButton(bool value)
    {
        _startWaveButton.enabled = value;
    }

    public void SendWaveButtonPressed()
    {
        ToggleSendWaveButton(false);
        ToggleSelectButtonVisibility(false);

        BuildMenuUIController.Instance.HideBuildMenu();
        CastleBuilderController.Instance.AssignCurrentSelectedPart(null);

        GameManager.Instance.ChangeState(GameState.Defending);

        TimeController.Instance.Initialize();
        EnemySpawningController.Instance.Initialize();
    }

    public void ToggleSelectButtonVisibility(bool visible, bool snap = false)
    {
        if (_windowMoveTween != null || _windowMoveTween.IsActive())
            _windowMoveTween.Kill(false);

        Vector2 newPosition = _startButtonShowingPosition;
        if (!visible)
            newPosition.y += _startButtonHiddenYOffset;

        float newTime = !snap ? StartButtonMoveTime : 0;

        _windowMoveTween = _startbuttonTransform.DOAnchorPos(newPosition, newTime).SetEase(_startButtonAnimationCurve);
    }
}