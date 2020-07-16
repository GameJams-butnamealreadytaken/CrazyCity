using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public GameObject Background;
    public GameObject PauseText;
    public GameObject WastedText;
    public GameObject ReplayText;

    public Text Player1Life;
    public Text Player1Score;
    public GameObject Player2Gui;
    public Text Player2Life;
    public Text Player2Score;

    public void DisplayBackground(bool bDisplay)
    {
        Background.SetActive(bDisplay);
    }

    public void DisplayPauseText(bool bDisplay)
    {
        PauseText.SetActive(bDisplay);
    }

    public void DisplayWastedText()
    {
        WastedText.SetActive(true);
    }

    public void DisplayReplayText()
    {
        ReplayText.SetActive(true);
    }

    public void DisplayPlayer1Infos(int life, int score)
    {
        Player1Life.text = life.ToString();
        Player1Score.text = score.ToString();
    }
    public void DisplayPlayer2Infos(int life, int score)
    {
        Player2Gui.SetActive(true);
        Player2Life.text = life.ToString();
        Player2Score.text = score.ToString();
    }
}
