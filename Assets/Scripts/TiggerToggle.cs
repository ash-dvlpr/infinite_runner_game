using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TiggerToggle : MonoBehaviour {
    //! ========================= Variables ==========================
    private bool _state;
    public bool State { get => _state; private set { _state = value; onStateChanged?.Invoke(); } }

    //? Components
    Collider2D _collider;

    //! ========================= Unity Code =========================
    void Awake() {
        // Get collider and set to trigger
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true; 
    }

    void OnTriggerEnter2D(Collider2D other) {
        State = !State;
    }

    // ===================== Custom Events Code ======================
    private event Action onStateChanged;
    public event Action OnStateChanged {
        add    { lock(this) { onStateChanged += value; } }
        remove { lock(this) { onStateChanged -= value; } }
    }
}
