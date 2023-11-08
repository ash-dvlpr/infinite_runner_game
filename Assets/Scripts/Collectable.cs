using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectable : MonoBehaviour {
    // ========================= Variables ==========================
    public bool IsCollected { get; private set; }
    [SerializeField] SO_CollectibleItem itemDefinition;
    public SO_CollectibleItem ItemDefinition { get => itemDefinition; }

    //? References
    Collider2D _collider;

    // ========================= Unity Code =========================
    void Awake() {
        // Get collider and set to trigger
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
    }

    private void Start() {
        IsCollected = false;
    }
    // ========================= Unity Code =========================
    void OnTriggerEnter2D(Collider2D other) {
        if (GameManager.GetState() == GameManager.GameState.InGame && other.CompareTag("Player")) { 
            Collect(other.GetComponent<PlayerController>());
        } 
    }

    // ========================= Custom Code ========================
    public void Collect(PlayerController player) {
        if (!IsCollected) { 
            IsCollected = true;
            ItemDefinition?.Collect(player);
            Destroy(this.gameObject);
        }
    }
}
