using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using static SO_AudioSettings;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Game Configuration/Audio Clip Settings", order = 2)]
public class SO_AudioSettings : ScriptableObject {
    // ========================= Variables ==========================
    [SerializeField] private List<ClipConfig> clipConfigs = new List<ClipConfig>();

    #region // ========================= Properties =========================
    public List<ClipConfig> ClipConfigs { get => clipConfigs; }

    #endregion

    // ========================= Custom Code ========================
    public enum ClipType : uint {
        Music  = 0,
        Effect = 1,
    }

    [System.Serializable]
    public class ClipConfig {
        [SerializeField]                     private ClipType  type;
        [SerializeField]                     private AudioClip clip;
        [SerializeField] [Range(0.0f, 1.0f)] private float     volume;

        public AudioClip Clip { get => clip; }
        public ClipType Type { get => type; }
        public float Volume { get => volume; }
    }

    // ===================== Outside Facing API ======================
    public ClipConfig FindClipConfig(AudioClip clip) {
        return ClipConfigs.Where((config) => { 
            return config.Clip == clip; 
        }).FirstOrDefault(null);
    }
}
