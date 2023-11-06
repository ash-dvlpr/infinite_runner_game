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
    }

    void Start() {
        LevelManager.ChunkSpawned(this);
    }

    void OnDestroy() {
        if (GameManager.GetState() == GameManager.GameState.InGame) { 
            LevelManager.ChunkDestroyed();
        }
    }
}
