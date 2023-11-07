using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Game Configuration/Difficulty Setting", order = 1)]
public class SO_DifficultySetting : ScriptableObject {
    // ========================= Variables ==========================
    [SerializeField] private float platformSpeed  = 5.0f;

    #region "Properties"
    public float PlatformSpeed { get => platformSpeed; }


    #endregion

    // ========================= Unity Code =========================
    // ========================= Custom Code =========================
}
