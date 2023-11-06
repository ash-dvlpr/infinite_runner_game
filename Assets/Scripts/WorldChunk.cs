using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public class WorldChunk : MonoBehaviour {
    //! ========================= Variables ==========================
    //? References & Components
    [SerializeField] private GameObject nextSpawnPoint;
    public GameObject NextSpawnPoint { get => nextSpawnPoint; }
    Rigidbody2D _rb;

    //! ========================= Unity Code =========================
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();

        // Subscribe events
        GameManager.Instance.OnGameStart += OnGameStart;
        GameManager.Instance.OnGameOver  += OnGameOver;
    }

    void Start() {
        LevelManager.ChunkSpawned(this);
    }

    void Update() {
        if (GameManager.GetState() == GameManager.GameState.InGame) { 
            _rb.velocity = Vector3.left * GameManager.Instance.PlatformSpeed;
        }
    }

    void OnDestroy() {
        LevelManager.ChunkDestroyed();
        // Unsubscribe events
        if (GameManager.Instance) {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameOver  -= OnGameOver;
        }
    }

    //! ========================= Custom Code ========================
    void OnGameStart() {
        _rb.simulated = true;
    }
    void OnGameOver() {
        _rb.simulated = false;
    }
}
