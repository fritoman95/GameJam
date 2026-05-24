using DG.Tweening;
using UnityEngine;

public class GamePlayUI : UIBase
{
    public static GamePlayUI Instance;

    [SerializeField]
    RectTransform _startbuttonTransform;

    Vector2 _startButtonShowingPosition;
    int _startButtonHiddenYOffset = 100;

    [SerializeField]
    AnimationCurve _startButtonAnimationCurve;
    const float StartButtonMoveTime = .35f;

    Tween _windowMoveTween;

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public override void EnablePanel()
    {
        _uiParentObject.SetActive(true);
    }

    public override void DisablePanel()
    {
        _uiParentObject.SetActive(false);
    }
}