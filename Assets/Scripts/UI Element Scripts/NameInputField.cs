using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class NameInputField : MonoBehaviour {

    public GameObject gameManager;
    public GameObject playGameButton;

    private void LockInput(InputField input) {
        string playerName = input.text;
        gameManager.GetComponent<GameManagerScript>().setPlayerName(playerName);
        if (playerName != "")
            playGameButton.GetComponent<Button>().interactable = true;
        else
            playGameButton.GetComponent<Button>().interactable = false;
    }

    private void Start() {
        InputField inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(delegate { LockInput(inputField); });
    }

}
