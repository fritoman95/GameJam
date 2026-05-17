using UnityEngine;

public class LevelOverUI : UIBase
{
    public static LevelOverUI Instance;

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void OnRestartButton()
    {

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