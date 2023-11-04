using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This singleton class act's as a "master" singleton (service locator) to integrate other systems. 
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    // ========================= Variables ==========================
    private GameState state = 0;
    private enum GameState : uint {
        MainMenu = 0,
        InGame   = 1,
        GameOver = 2,
    }

    // ========================= Unity Code =========================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) Destroy(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }
    
    // ======================= Game State Code =======================


    // ========================= Custom Code =========================

}
