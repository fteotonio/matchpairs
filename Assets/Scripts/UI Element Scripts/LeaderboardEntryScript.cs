using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryScript : MonoBehaviour {
    
    public GameObject playerNameText;
    public GameObject scoreText;
    public GameObject movesText;
    public GameObject timeText;

    public void UpdateEntry(string playerName, int score, int moves, System.TimeSpan time, int rank) {
        playerNameText.GetComponent<Text>().text = rank + ". " + playerName;
        scoreText.GetComponent<Text>().text = "Score: " + score;
        movesText.GetComponent<Text>().text = "Moves: " + moves;
        timeText.GetComponent<Text>().text = "Time elapsed: " + time.ToString("mm\\:ss");
    }

}
