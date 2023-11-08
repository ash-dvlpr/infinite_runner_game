using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OnContactDamage : MonoBehaviour {
    //! ========================= Unity Code =========================
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            var player = other.GetComponent<PlayerController>();
            player.DealDamage(1);
        }
    }
}
