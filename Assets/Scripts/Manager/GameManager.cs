using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, GameOver, Victory}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Status")]
    [SerializeField] private GameState currentState;

    public GameState CurrentState => currentState;

    //Events
    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ChangeState(GameState.Playing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing) ChangeState(GameState.Paused);
            else if (currentState == GameState.Paused) ChangeState(GameState.Playing);
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case GameState.Playing:
                Time.timeScale = 1;
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            case GameState.GameOver:
            case GameState.Victory:
                Time.timeScale = 1;
                break;
        }

        OnStateChanged?.Invoke(currentState);
    }

    public void SetVictory() => ChangeState(GameState.Victory);
    public void SetGameOver() => ChangeState(GameState.GameOver);

    public void RestartGame() => SceneManager.LoadScene("SampleScene");

    public void QuitGame()
    {
        #if UNITY_WEBGL
        SceneManager.LoadScene("MainMenu");
        #else
        Application.Quit();
        #endif
    }
}
