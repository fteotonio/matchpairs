using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitToTitleScreenScript : MonoBehaviour {

    public GameObject gameManager;
    public GameObject introScreen;
    public GameObject gameScreen;
    public GameObject leaderboardScreen;
    public GameObject exitToTitleScreen;

    public void PressYes() {
        gameManager.GetComponent<GameManagerScript>().UpdateCurrentSession();
        introScreen.SetActive(true);
        gameScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        exitToTitleScreen.SetActive(false);
    }

    public void PressNo() {
        gameManager.GetComponent<GameManagerScript>().PauseGame(false);
        exitToTitleScreen.SetActive(false);
    }

    public void Activate() {
        gameManager.GetComponent<GameManagerScript>().PauseGame(true);
        exitToTitleScreen.SetActive(true);
    }

}
