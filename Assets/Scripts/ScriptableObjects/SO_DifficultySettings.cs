using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Game Configuration/Difficulty Settings", order = 1)]
public class SO_DifficultySettings : ScriptableObject {
    // ========================= Variables ==========================
    [SerializeField] private float platformSpeed   = 5.0f;
    [SerializeField] private int   playerMaxHealth = 6;


    #region // ========================= Properties =========================
    public float PlatformSpeed { get => platformSpeed; }
    public int PlayerMaxHealth { get => playerMaxHealth; }

    #endregion

    // ========================= Unity Code =========================
    // ========================= Custom Code ========================
}
