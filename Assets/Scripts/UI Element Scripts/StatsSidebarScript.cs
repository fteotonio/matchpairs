using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsSidebarScript : MonoBehaviour {

    public GameObject playerNameText;
    public GameObject movesText;
    public GameObject timeText;

    public void UpdatePlayerNameText(string playerName) {
        playerNameText.GetComponent<Text>().text = playerName;
    }

    public void UpdateMovesText(int moves) {
        movesText.GetComponent<Text>().text = "Moves: " + moves;
    }

    public void UpdateTimeText(System.TimeSpan time) {
        timeText.GetComponent<Text>().text = "Time elapsed: " + time.ToString("mm\\:ss");
    }
}
