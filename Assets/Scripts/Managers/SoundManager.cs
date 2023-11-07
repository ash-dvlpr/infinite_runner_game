using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    // ========================= Variables ==========================
    [SerializeField] SO_AudioSettings audioSettings;


    //? References  & Components
    // TODO: AudioSource pooling
    AudioSource audioSource;

    // ========================= Unity Code =========================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    // ===================== Custom Events Code ======================
    // ===================== Outside Facing API ======================
    public static SO_AudioSettings GetAudioSettings => Instance?.audioSettings;
}
