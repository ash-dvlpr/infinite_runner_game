using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour {

    //! ========================= Variables ==========================
    //? References & Components
    Rigidbody2D rb;
    TiggerToggle triggerToggle;

    //! ========================= Unity Code =========================
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        triggerToggle = rb.GetComponentInChildren<TiggerToggle>();

        
        triggerToggle.OnStateChanged += Flip;
    }

    void Update() {
        if (GameManager.GetState() == GameManager.GameState.InGame) {
            rb.velocity = new Vector2(GameManager.Instance.PlatformSpeed * (triggerToggle.State ? 1 : -1), rb.velocity.y);
        }
    }

    private void OnDestroy() {
        if (triggerToggle) {
            triggerToggle.OnStateChanged -= Flip;
        }
    }

    //! ========================= Custom Code ========================
    void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

}
