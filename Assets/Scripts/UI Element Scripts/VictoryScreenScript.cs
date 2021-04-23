using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class VictoryScreenScript : MonoBehaviour {

    public GameObject gameManager;
    public GameObject gameScreen;
    public GameObject introScreen;
    public GameObject victoryScreen;

    public GameObject victoryTimeText;
    public GameObject victoryMovesText;
    public GameObject victoryScoreText;

    public void UpdateScreen(int score, int moves, System.TimeSpan time) {
        victoryTimeText.GetComponent<Text>().text = "Time elapsed: " + time.ToString("mm\\:ss");
        victoryMovesText.GetComponent<Text>().text = "Moves: " + moves;
        victoryScoreText.GetComponent<Text>().text = "Final Score: " + score;
    }

    public void PressPlayAgain() {
        victoryScreen.SetActive(false);
        gameManager.GetComponent<GameManagerScript>().StartGame();
    }

    public void PressExit() {
        introScreen.SetActive(true);
        gameScreen.SetActive(false);
        victoryScreen.SetActive(false);
    }

    public void Activate() {
        gameObject.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

}
