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

    void ShowBuildMenu()
    {
        _showingBuildMenu = true;

        //Move the window into place

        ToggleBuildingUIInteractivity(true);
    }

    void ToggleBuildingUIInteractivity(bool value)
    {
        //Turn on the window and allow the player to grab the parts
        CastleBuilderController.Instance.ListOfParts.ForEach(x => x.ToggleClickability(value));
    }

    void HideBuildMenu()
    {
        _currentHidingTimer = 0;
        _showingBuildMenu = false;

        UpdateCurrentBuildPartsUI(null);
        ToggleBuildingUIInteractivity(false);

        //Move the window down
    }

    public void UpdateCurrentBuildPartsUI(PartInBuildMenu part)
    {
        _subPanelController.UpdateCurrentBuildPartsUI(part);
    }
}