using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Outline P1Text;
    public GameObject P1Gui;
    public Outline P2Text;
    public GameObject P2Gui;

    bool b2Player = false;

    private void RedrawGui()
    {
        P1Text.enabled = !b2Player;
        P1Gui.SetActive(!b2Player);
        P2Text.enabled = b2Player;
        P2Gui.SetActive(b2Player);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            b2Player = !b2Player;
            RedrawGui();
        }

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            StaticDatas.b2Player = b2Player;
            SceneManager.LoadScene("Level1_little", LoadSceneMode.Single);
        }
    }
}
