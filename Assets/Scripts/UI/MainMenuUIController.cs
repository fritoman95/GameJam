using DG.Tweening;
using UnityEngine;

public class MainMenuUIController : UIBase
{
    public static MainMenuUIController Instance;

    [SerializeField]
    GameObject _mainMenuGameObject;

    float _growPanelTime = .75f;
    float _growFadeOutTime = .5f;

    [SerializeField]
    AnimationCurve _mainMenuGrowCurve;
    [SerializeField]
    AnimationCurve _mainMenuShrinkCurve;

    Tween _growingPanelTween;

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void ShowMainMenu()
    {
        _growingPanelTween?.Kill(true);
        _growingPanelTween = _mainMenuGameObject.transform.DOScale(Vector3.one, _growPanelTime).SetEase(_mainMenuGrowCurve);
    }

    void HideMainMenu(bool snap = false)
    {
        _growingPanelTween?.Kill(true);

        if(snap)
        {
            _mainMenuGameObject.transform.localScale = Vector3.zero;
            return;
        }

        _growingPanelTween = _mainMenuGameObject.transform.DOScale(Vector3.zero, _growFadeOutTime).SetEase(_mainMenuShrinkCurve);
    }

    public void StartGame()
    {
        HideMainMenu();
        GameManager.Instance.StartGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public override void EnablePanel()
    {
        ShowMainMenu();

        _uiParentObject.SetActive(true);
    }

    public override void DisablePanel()
    {
        HideMainMenu();

        _uiParentObject.SetActive(false);
    }
}