using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Playing,
    GameOver,
    Victory
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Trạng Thái Hiện Tại")]
    [SerializeField] private GameState currentState = GameState.Playing;
    public GameState CurrentState => currentState;

    public event Action<GameState> OnGameStateChanged;

    private PlayerMovement player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.OnDeath += HandlePlayerDeath;
        }

        SetState(GameState.Playing);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnDeath -= HandlePlayerDeath;
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"[GameManager] Trạng thái game chuyển sang: {newState}");
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandlePlayerDeath()
    {
        SetState(GameState.GameOver);
    }


    public void TriggerVictory()
    {
        SetState(GameState.Victory);
    }


    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
