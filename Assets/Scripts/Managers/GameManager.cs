using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

    //? State
#if UNITY_EDITOR
    [SerializeField] private bool overrideSceneLoading = false;
#endif
    [SerializeField] SO_DifficultySetting difficultySettings;
    public float PlatformSpeed { get => difficultySettings?.PlatformSpeed ?? 5.0f; }

    private GameState state = 0;
    public enum GameState : int {
        None     = SCENE_ID_MAIN,
        MainMenu = SCENE_ID_MENU,
        InGame   = SCENE_ID_GAME,
        GameOver = SCENE_ID_GAMEOVER,
    }

    //? Scores
    private int   _coins = 0;
    private float _distanceTraveled = 0;

    public float DistanceTraveled {
        get => _distanceTraveled;
        private set { _distanceTraveled = value; NotifyScoreChanged(); }
    }
    public float Score { 
        get => _distanceTraveled + (_coins * difficultySettings?.CoinScoreValue ?? 1);
    }
    public int Coins { 
        get => _coins; 
        private set { _coins = value; NotifyScoreChanged(); } 
    }
    public float HighScore {
        get => PlayerPrefs.GetFloat("highscore", 0);
        private set {
            PlayerPrefs.SetFloat("highscore", value); 
            NotifyScoreChanged();
        }
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
        if (GameState.None == state) {
            // Load Menu and background Game Scenes
#if UNITY_EDITOR
            if (!overrideSceneLoading) {
                LoadScene(SCENE_ID_MENU);
                LoadScene(SCENE_ID_GAME, true);
            }
            else {
                SetActiveScene(SCENE_ID_GAME);
            }
#else
            LoadScene(SCENE_ID_MENU);
            LoadScene(SCENE_ID_GAME, true);
#endif
        }

        else {
            if (GameState.GameOver == state) {
                // Remove GameOver screen
                UnloadScene(SCENE_ID_GAMEOVER);
            }
         
            // Load Game and Menu screen 
            LoadScene(SCENE_ID_MENU);
            ReloadScene(SCENE_ID_GAME, true);
        }

        return GameState.MainMenu;
    }
    private GameState HandleToInGame() {
        Coins = 0;
        DistanceTraveled = 0;

        if (GameState.MainMenu == state) {
            // Remove Menu screen
            UnloadScene(SCENE_ID_MENU);
        }
        else if (GameState.GameOver == state) {
            // Remove GameOver screen and reload Game
            UnloadScene(SCENE_ID_GAMEOVER);
            ReloadScene(SCENE_ID_GAME, true);
        }

        DelayMethod(NotifyGameStarted);
        return GameState.InGame;
    }
    private GameState HandleToGameOver() {
        if(GameState.InGame == state) {
            // Load GameOver screen
            LoadScene(SCENE_ID_GAMEOVER);

            // Save HighScore
            if (Score > HighScore) { 
                HighScore = Score; 
            }
        }

        NotifyGameOver();
        return GameState.GameOver;
    }

    private static void UnloadScene(int sceneBuildIndex) {
        SceneManager.UnloadSceneAsync(sceneBuildIndex);
    }
    private static void LoadScene(int sceneBuildIndex, bool setActive = false) {
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Additive);
        if (setActive) Instance?.SetActiveScene(sceneBuildIndex);
    }
    private static void ReloadScene(int sceneBuildIndex, bool setActive = false) {
        #pragma warning disable CS0618 // Type or member is obsolete
        SceneManager.UnloadScene(sceneBuildIndex);
        #pragma warning restore CS0618 // Type or member is obsolete
        LoadScene(sceneBuildIndex, setActive);
    }
 
    private void SetActiveScene(int sceneBuildIndex) {
        DelayMethod(() => { SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex)); });
    }

    // ===================== Custom Events Code ======================
    private void NotifyGameStarted() => onGameStart?.Invoke();
    private event Action onGameStart;
    public event Action OnGameStart {
        add    { lock(this) { onGameStart += value; } }
        remove { lock(this) { onGameStart -= value; } }
    }

    private void NotifyGameOver() => onGameOver?.Invoke();
    private event Action onGameOver;
    public event Action OnGameOver {
        add    { lock(this) { onGameOver += value; } }
        remove { lock(this) { onGameOver -= value; } }
    }

    private void NotifyScoreChanged() => onScoreChanged?.Invoke();
    private event Action onScoreChanged;
    public event Action OnScoreChanged {
        add    { lock(this) { onScoreChanged += value; } }
        remove { lock(this) { onScoreChanged -= value; } }
    }

    // ===================== Outside Facing API ======================
    /// <summary>
    /// Delays a method invocation aproximatelly 1 frame.
    /// </summary>
    /// <param name="method">Code to execute.</param>
    public static void DelayMethod(Action method) {
        Instance?.StartCoroutine(DelayMethodCoroutine(method));
    }
    private static IEnumerator DelayMethodCoroutine(Action method) {
        yield return 0;
        method.Invoke();
        yield break;
    }

    public static GameState GetState() => Instance?.state ?? GameState.None;
    public static void UpdateDistanceTraveled(float newDistanceTraveled) {
        if (Instance) {
            Instance.DistanceTraveled = newDistanceTraveled;
        }
    }
    public static void AddCoins(int coinsAmount) {
        if (Instance) {
            Instance.Coins += coinsAmount;
        }
    }
    public static void RestartGame() {
        Instance?.TryChangeGameState(GameState.InGame);
    }
    public static void GameOver() {
        Instance?.TryChangeGameState(GameState.GameOver);
    }
    public static void ToMainMenu() {
        Instance?.TryChangeGameState(GameState.MainMenu);
    }
}