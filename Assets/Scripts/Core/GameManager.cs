using UnityEngine;

public enum GameState
{
    NotPlaying,
    Building,
    Defending,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static GameState CurrentState;

    public EconomyController Economy;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        ChangeState(GameState.NotPlaying);

        Economy.Initialize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            StartGame();
    }

    public void StartGame()
    {
        ChangeState(GameState.Building);

        EconomyUI.Instance.Initialize();
    }

    public void ChangeState(GameState desiredGameState)
    {
        CurrentState = desiredGameState;
    }

}