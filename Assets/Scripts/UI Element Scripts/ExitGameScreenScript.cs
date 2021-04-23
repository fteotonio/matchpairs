using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameScreenScript : MonoBehaviour
{
    public GameObject exitGameScreen;

    public void PressYes() {
        Application.Quit();
    }

    public void PressNo() {
        exitGameScreen.SetActive(false);
    }

    public void Activate() {
        exitGameScreen.SetActive(true);
    }
}
