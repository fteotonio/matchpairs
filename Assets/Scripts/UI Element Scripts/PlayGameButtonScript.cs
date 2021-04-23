using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameButtonScript : MonoBehaviour {

    public GameObject gameManager;
    public GameObject introScreen;
    public GameObject gameScreen;

    public void StartGameScreen() {
        gameManager.GetComponent<GameManagerScript>().StartGame();
        introScreen.SetActive(false);
        gameScreen.SetActive(true);
    }

}
