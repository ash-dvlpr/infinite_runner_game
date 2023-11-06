using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    // ========================= Variables ==========================
    //? References
    [SerializeField] List<GameObject> chunkPrefabs;
    GameObject spawnPoint;

    // ========================= Unity Code =========================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            Instance = this;
        }
    }

    // ========================= Custom Code =========================
    private void SpawnNextChunk() {
        var variant = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        var chunk = Instantiate(variant, spawnPoint.transform.position, Quaternion.identity);

        spawnPoint = chunk.GetComponent<WorldChunk>().NextSpawnPoint;
    }

    // ===================== Custom Events Code ======================
    private void NotifyChunkSpawned(WorldChunk newChunk) => onChunkSpawned?.Invoke(newChunk);
    public event Action<WorldChunk> onChunkSpawned;

    private void NotifyChunkDestroyed() => onChunkDestroyed?.Invoke();
    public event Action onChunkDestroyed;

    // ===================== Outside Facing API ======================
    public static void RequestNextChunk() => Instance?.SpawnNextChunk();
    public static void ChunkSpawned(WorldChunk newChunk) {
        // Cache first chunk spawned
        if (null != Instance && null == Instance.spawnPoint) { 
            Instance.spawnPoint = newChunk?.NextSpawnPoint ?? Instance.spawnPoint;
        }
        Instance?.NotifyChunkSpawned(newChunk);
    }
    public static void ChunkDestroyed() {
        if (GameManager.GetState() == GameManager.GameState.InGame) {
            Instance?.SpawnNextChunk();
        }
        Instance?.NotifyChunkDestroyed();
    }
}