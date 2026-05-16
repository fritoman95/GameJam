using System;
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
    [SerializeField]
    GameState TEMP_currentState;

    public EconomyController Economy;

    [SerializeField]
    EndGameCollider _endGameCollider;

    public Action OnGameOver;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        _endGameCollider.OnGameEnd += () => ChangeState(GameState.GameOver);
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
        if (CurrentState == desiredGameState)
            return;

        CurrentState = desiredGameState;
        TEMP_currentState = CurrentState;

        if (CurrentState == GameState.GameOver)
            OnGameOver.Invoke();
    }
}