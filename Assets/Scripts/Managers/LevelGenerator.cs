using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    // ========================= Variables ==========================
    //? References
    [SerializeField] List<GameObject> chunkPrefabs;
    [SerializeField] Vector2 startSpawnPosition = new Vector2(9,0);
    Vector2 currentSpawnPosition;

    // ========================= Unity Code =========================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            Instance = this;
        }
    }

    void Start() {
        currentSpawnPosition = startSpawnPosition;
    }

    // ===================== Outside Facing API ======================
    // Spawn a block, update the currentSpawnPosition with the tagged next spawn position (child game object) of the spawned block

    // ===================== Custom Events Code ======================
}
