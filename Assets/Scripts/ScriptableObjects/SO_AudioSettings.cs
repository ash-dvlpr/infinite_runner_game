using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Game Configuration/Audio Clip Settings", order = 2)]
public class SO_AudioSettings : ScriptableObject {
    // ========================= Variables ==========================
    [SerializeField] private List<ClipConfig> clipConfigs = new List<ClipConfig>();

    #region // ========================= Properties =========================
    public List<ClipConfig> ClipConfigs { get => clipConfigs; }

    #endregion

    // ========================= Custom Code ========================
    [System.Serializable]
    public class ClipConfig {
        [SerializeField]                     private AudioClip clip;
        [SerializeField] [Range(0.0f, 1.0f)] private float     volume;

        public AudioClip Clip { get => clip; }
        public float Volume { get => volume; }
    }
}
