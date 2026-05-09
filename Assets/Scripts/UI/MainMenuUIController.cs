using DG.Tweening;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
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

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);

        HideMainMenu(snap: true);
    }

    void Start()
    {
        ShowMainMenu();
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
}
