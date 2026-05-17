using DG.Tweening;
using TMPro;
using UnityEngine;

public class EconomyUI : UIBase
{
    public static EconomyUI Instance;

    [SerializeField]
    TextMeshProUGUI _currentMoneyText;

    [SerializeField]
    RectTransform _moneyTabGameObject;

    Vector2 _moneyTabShowingPosition;
    int _moneyTabHiddenYOffset = 100;

    [SerializeField]
    AnimationCurve _buildMenuMoveAnimationCurve;
    const float BuildMenuMoveTime = .35f;

    Tween _windowMoveTween;

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        _moneyTabShowingPosition = _moneyTabGameObject.anchoredPosition;
    }

    public void UpdateMoneyAmount()
    {
        _currentMoneyText.text = GameManager.Instance.Economy.CurrentMoneyValue.ToString();
    }

    public void ToggleMoneyTabVisibility(bool visible, bool snap = false)
    {
        if (_windowMoveTween != null || _windowMoveTween.IsActive())
            _windowMoveTween.Kill(false);

        Vector2 newPosition = _moneyTabShowingPosition;
        if (!visible)
            newPosition.y += _moneyTabHiddenYOffset;

        float newTime = !snap ? BuildMenuMoveTime : 0;

        _windowMoveTween = _moneyTabGameObject.DOAnchorPos(newPosition, newTime).SetEase(_buildMenuMoveAnimationCurve);
    }

    public override void EnablePanel()
    {
        //Set the current money amount
        UpdateMoneyAmount();

        //drop in money tab
        ToggleMoneyTabVisibility(true);

        _uiParentObject.SetActive(true);
    }

    public override void DisablePanel()
    {
        _uiParentObject.SetActive(false);
    }
}