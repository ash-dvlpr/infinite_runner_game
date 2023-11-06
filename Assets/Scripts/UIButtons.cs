using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour {
    public void RestartGame() => GameManager.RestartGame();
    public void ToMainMenu() => GameManager.ToMainMenu();
}
