using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is in charge of managing the game's state and doing all the scene loading/unloading. <br/><br/>
/// 
/// On top of that this <b>master</b> controller uses an Event system to integrate with other scripts and avoid sincronization errors.
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    // ========================= Variables ==========================
    private const int SCENE_ID_MAIN     = 0;
    private const int SCENE_ID_MENU     = 1;
    private const int SCENE_ID_GAME     = 2;
    private const int SCENE_ID_GAMEOVER = 3;

    //? State & Scores
    [SerializeField] private float platformSpeed = 5.0f;
    public float PlatformSpeed { get => platformSpeed; }

    private GameState state = 0;
    public enum GameState : int {
        None     = SCENE_ID_MAIN,
        MainMenu = SCENE_ID_MENU,
        InGame   = SCENE_ID_GAME,
        GameOver = SCENE_ID_GAMEOVER,
    }

    // ========================= Unity Code =========================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }

    void Start() {
        TryChangeGameState(GameState.MainMenu);    
    }

    void OnDestroy() {
        //state = GameState.None;    
    }

    // ======================= Game State Code =======================
    private void TryChangeGameState(GameState newState) { 
        if (state == newState) return;

        // Finite State Machine to handle possible state changes
        state = newState switch {
            GameState.MainMenu => HandleToMenu(),
            GameState.InGame   => HandleToInGame(),
            GameState.GameOver => HandleToGameOver(),
            _                  => HandleDefault(newState),
        };

    }

    private GameState HandleDefault(GameState newState) {
        Debug.LogError($"TryChangeGameState() -> Unimplemented handling for state transition: '{state}'=>'{newState}'.");
        return this.state;
    }

    private GameState HandleToMenu() {
        if (GameState.None == state) {
            // Load Menu and background Game Scenes
            #if !UNITY_EDITOR
            SceneManager.LoadScene(SCENE_ID_MENU, LoadSceneMode.Additive);
            SceneManager.LoadScene(SCENE_ID_GAME, LoadSceneMode.Additive);
            #endif
        }

        else {
            // Load Menu Screen
            SceneManager.LoadScene(SCENE_ID_MENU, LoadSceneMode.Additive);
            // Reload Game
            #pragma warning disable CS0618
            SceneManager.UnloadScene(SCENE_ID_GAME);
            SceneManager.LoadScene(SCENE_ID_GAME, LoadSceneMode.Additive);
        }

        return GameState.MainMenu;
    }
    private GameState HandleToInGame() {
        if (GameState.MainMenu == state) {
            // Remove MainMenu Screen
            #pragma warning disable CS0618
            SceneManager.UnloadScene(SCENE_ID_MENU);
        }

        else if (GameState.GameOver == state) {
            // Remove GameOver Screen
            #pragma warning disable CS0618
            SceneManager.UnloadScene(SCENE_ID_GAMEOVER);
            // Reload Game
            #pragma warning disable CS0618
            SceneManager.UnloadScene(SCENE_ID_GAME);
            SceneManager.LoadScene(SCENE_ID_GAME, LoadSceneMode.Additive);
        }

        // Generate the first Chunks
        LevelManager.RequestNextChunk();
        LevelManager.RequestNextChunk();

        NotifyGameStarted();
        return GameState.InGame;
    }
    private GameState HandleToGameOver() {
        NotifyGameOver();
        return GameState.GameOver;
    }

    // ===================== Custom Events Code ======================
    private void NotifyGameStarted() => onGameStart?.Invoke(); 
    public event Action onGameStart;
    
    private void NotifyGameOver() => onGameOver?.Invoke(); 
    public event Action onGameOver;

    // ===================== Outside Facing API ======================
    public static GameState GetState() => Instance?.state ?? GameState.None;
    public static void RestartGame() {
        Instance?.TryChangeGameState(GameState.InGame);
    }
    public static void GameOver() {
        Instance?.TryChangeGameState(GameState.GameOver);
    }
}