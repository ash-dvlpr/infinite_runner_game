using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is in charge of managing the game's state and doing all the scene loading/unloading.
/// 
/// Also, this singleton class act's as a <b>service locator</b> to integrate other systems. <br/>
/// 
/// 
/// <br/>TODO: Game will start on an empty scene with a GameManager with state 0. And from there load the main menu and manage unloading/loading new scenes.
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    // ========================= Variables ==========================
    private const int SCENE_ID_MAIN     = 0;
    private const int SCENE_ID_MENU     = 1;
    private const int SCENE_ID_GAME     = 2;
    private const int SCENE_ID_GAMEOVER = 3;

    //? State & Scores
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
        // Initial load 
        if (GameState.None == state) {
            #if !UNITY_EDITOR
            SceneManager.LoadScene(SCENE_ID_MENU, LoadSceneMode.Additive);
            SceneManager.LoadScene(SCENE_ID_GAME, LoadSceneMode.Additive);
            #endif
        }
        else {
            SceneManager.LoadScene(SCENE_ID_MENU, LoadSceneMode.Additive);
        }
        return GameState.MainMenu;
    }
    private GameState HandleToInGame() {
        // Hide Menus
        if (GameState.MainMenu == state) {
            SceneManager.UnloadScene(SCENE_ID_MENU);
            TriggerGameStartEvents();
        }

        else if (GameState.GameOver == state) {
            SceneManager.UnloadScene(SCENE_ID_GAMEOVER);
            TriggerGameRestartEvents();
        }

        return GameState.InGame;
    }
    private GameState HandleToGameOver() {
        TriggerGameOverEvents();
        return GameState.GameOver;
    }

    // ===================== Outside Facing API ======================
    public static GameState GetState() => Instance.state;
    public static void RestartGame() {
        Instance.TryChangeGameState(GameState.InGame);
    }
    public static void GameOver() {
        Instance.TryChangeGameState(GameState.GameOver);
    }

    // ===================== Custom Events Code ======================
    public event Action onGameStart;
    private void TriggerGameStartEvents() {
        Instance?.onGameStart(); 
    }
    
    public event Action onGameOver;
    private void TriggerGameOverEvents() {
        Instance?.onGameOver(); 
    }

    public event Action onGameRestart;
    private void TriggerGameRestartEvents() {
        Instance?.onGameRestart(); 
    }

}