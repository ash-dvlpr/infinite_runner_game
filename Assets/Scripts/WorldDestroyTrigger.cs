using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDestroyTrigger : MonoBehaviour {
    //! ========================= Unity Code =========================
    void OnTriggerEnter2D(Collider2D other) {
        // HACK: Should handle differently if pooling the game object
        Destroy(other.gameObject);
    }
}
