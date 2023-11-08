using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class PlayerUI : MonoBehaviour {
    private static string SCORE = "SCORE", HIGHSCORE = "RECORD";
    //! ========================= Variables ==========================
    //? References & Components
    [Header("General Configuration")]
    [SerializeField] bool isGameOverScreen;
    [SerializeField] GameObject uiRoot;

    [Header("UI Text Fields")]
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text highscore;
    [SerializeField] TMP_Text coinCounter;

    //! ========================= Unity Code =========================
    void Awake() {
        // Subscribe events
        GameManager.Instance.OnGameStart    += OnGameStart;
        GameManager.Instance.OnGameOver     += OnGameOver;
        GameManager.Instance.OnScoreChanged += UpdateScore;
    }

    void Start() {
        UpdateScore();
        uiRoot?.SetActive(false);
    }

    void OnDestroy() {
        // Unsubscribe events
        if (GameManager.Instance) {
            GameManager.Instance.OnGameStart    -= OnGameStart;
            GameManager.Instance.OnGameOver     -= OnGameOver;
            GameManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }

    //! ========================= Custom Code ========================
    void UpdateScore() {
        score?.SetText($"{SCORE}\n{Mathf.FloorToInt(GameManager.Instance.Score)}");
        coinCounter?.SetText($"{GameManager.Instance.Coins}");
        highscore?.SetText(
            GameManager.Instance.HighScore > 0 
            ? $"{HIGHSCORE}\n{Mathf.FloorToInt(GameManager.Instance.HighScore)}" : ""
        );
    }

    void OnGameStart() {
        uiRoot?.SetActive(!isGameOverScreen);
    }
    void OnGameOver() {
        uiRoot?.SetActive(isGameOverScreen);
    }
}
