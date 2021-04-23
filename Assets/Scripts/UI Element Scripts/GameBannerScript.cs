using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBannerScript : MonoBehaviour
{
    public GameObject leaderboardScreen;
    public GameObject gameScreen;
    public GameObject gameManager;

    public void ChangeTabToLeaderboard() {
        leaderboardScreen.SetActive(true);
        gameManager.GetComponent<GameManagerScript>().PauseGame(true);
        gameScreen.SetActive(false);
    }

    public void ChangeTabToGame() {
        gameScreen.SetActive(true);
        gameManager.GetComponent<GameManagerScript>().PauseGame(false);
        leaderboardScreen.SetActive(false);
    }
}
