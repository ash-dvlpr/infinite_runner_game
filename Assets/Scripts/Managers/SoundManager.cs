using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    // ========================= Variables ==========================
    [SerializeField] SO_AudioSettings audioSettings;

    [Header("Game Music")]
    [SerializeField] AudioClip menuTheme;
    [SerializeField] AudioClip mainTheme;
    [SerializeField] AudioClip gameOverTheme;

    //? References  & Components
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource soundAudioSource;
    // TODO: AudioSource pooling

    // ========================= Unity Code =========================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        // Subscribe events
        if (GameManager.Instance) {
            GameManager.Instance.OnMenuLoaded += OnMenuLoaded;
            GameManager.Instance.OnGameStart  += OnGameStart;
            GameManager.Instance.OnGameOver   += OnGameOver;
        }
    }

    void OnDestroy() {
        // Unsubscribe events
        if (GameManager.Instance) {
            GameManager.Instance.OnMenuLoaded -= OnMenuLoaded;
            GameManager.Instance.OnGameStart  -= OnGameStart;
            GameManager.Instance.OnGameOver   -= OnGameOver;
        }
    }


    // ===================== Custom Events Code ======================
    void OnMenuLoaded() {
        GameManager.DelayMethod(() => { PlayClip(menuTheme); });
    }
    void OnGameStart() {
        GameManager.DelayMethod(() => { PlayClip(mainTheme); });
    }
    void OnGameOver() {
        GameManager.DelayMethod(() => { PlayClip(gameOverTheme); });
    }



    // ===================== Outside Facing API ======================
    public static SO_AudioSettings GetAudioSettings => Instance?.audioSettings;

    public static void PlayClip(AudioClip clip) {
        if (Instance && clip) {
            var config = Instance.audioSettings.FindClipConfig(clip);
            if (null != config) { 
                switch (config.Type) {
                    case SO_AudioSettings.ClipType.Music: 
                        Instance.musicAudioSource.Stop();
                        Instance.musicAudioSource.clip = clip;
                        Instance.musicAudioSource.volume = config.Volume;
                        Instance.musicAudioSource.loop = config.Loop;
                        Instance.musicAudioSource.Play();
                        break;
                    case SO_AudioSettings.ClipType.Sound:
                        Instance.soundAudioSource.PlayOneShot(clip, config.Volume);
                        break;
                }
            } else {
                Debug.LogError($"SoundManager: Couldn't locate settings for audio clip '{clip}'");
            }
        }
    }
}
