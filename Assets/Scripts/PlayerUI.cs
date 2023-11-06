using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class PlayerUI : MonoBehaviour {
    private static string SCORE = "SCORE", HIGHSCORE = "RECORD";
    //! ========================= Variables ==========================
    //? References
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text highscore;
    [SerializeField] TMP_Text coinCounter;

    //! ========================= Unity Code =========================
    void Awake() {
        // Subscribe events
        GameManager.Instance.OnGameStart += OnGameStart;
        GameManager.Instance.OnGameOver  += OnGameOver;
    }

    void OnDestroy() {
        // Unsubscribe events
        if (GameManager.Instance) {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameOver  -= OnGameOver;
        }
    }

    //! ========================= Custom Code ========================
    void OnGameStart() {
        score.enabled = true;
        // TODO
    }
    void OnGameOver() {
        score.enabled = false;
        // TODO
    }
}
