using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UIPanel
{
    MainMenu,
    BuildMenu,
    Gameplay,
    Economy,
    LevelOver
}

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public UIPanel CurrentPanel;

    [SerializeField]
    MainMenuUIController _mainmenutUI;
    [SerializeField]
    GamePlayUI _gameplayUI;
    [SerializeField]
    BuildMenuUIController _buildMenuUI;
    [SerializeField]
    EconomyUI _economyUI;
    [SerializeField]
    LevelOverUI _levelOverUI;

    public static MainMenuUIController MainMenuUI;
    public static GamePlayUI GameplayUI;
    public static BuildMenuUIController BuildMenuUI;
    public static EconomyUI EconomyUI;
    public static LevelOverUI LevelOverUI;

    public Dictionary<UIPanel, UIBase> UIPanels = new Dictionary<UIPanel, UIBase>()
    {
        {UIPanel.MainMenu, MainMenuUI},
        {UIPanel.Gameplay, GameplayUI},
        {UIPanel.BuildMenu, BuildMenuUI },
        {UIPanel.Economy, EconomyUI },
        {UIPanel.LevelOver, LevelOverUI},
    };

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        InitializeMenus();
    }

    void Start()
    {
        ChangeCurrentPanel(UIPanel.MainMenu);
    }

    public void ChangeCurrentPanel(UIPanel panel)
    {
        UIPanels.TryGetValue(panel, out UIBase currentPanel);
        currentPanel.DisablePanel();

        CurrentPanel = panel;

        UIPanels.TryGetValue(panel, out UIBase nextPanel);
        nextPanel.EnablePanel();
    }

    void InitializeMenus()
    {
        MainMenuUI = _mainmenutUI;
        GameplayUI = _gameplayUI;
        BuildMenuUI = _buildMenuUI;
        EconomyUI = _economyUI;
        LevelOverUI = _levelOverUI;

        UIPanels = new Dictionary<UIPanel, UIBase>()
            {
                {UIPanel.MainMenu, MainMenuUI},
                {UIPanel.Gameplay, GameplayUI},
                {UIPanel.BuildMenu, BuildMenuUI },
                {UIPanel.Economy, EconomyUI },
                {UIPanel.LevelOver, LevelOverUI},
            };

        for (int i = 0; i < UIPanels.Count; i++)
        {
            UIPanel currentKey = UIPanels.Keys.ToList()[i];
            UIPanels.TryGetValue(currentKey, out UIBase currentBase);

            if (currentBase == null)
                currentBase = FindFirstObjectByType<EconomyUI>();
            
            if(currentBase == null)
                continue;

            currentBase.Initialize();
            currentBase.DisablePanel();
        }
    }
}