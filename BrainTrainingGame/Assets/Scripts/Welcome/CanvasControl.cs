using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour
{
    public Canvas RegisterCanvas;
    //public Canvas MsgBoxCanvas;
    public string PlayerName = "Guest";
    public Text WelcomeText;

    private void Start()
    {
        RegisterCanvas.gameObject.SetActive(false);
        //MsgBoxCanvas.gameObject.SetActive(false);
        WelcomeText.text = "Welcome, " + PlayerName;
    }

    public void SetPlayerName(string playerName)
    {
        PlayerName = playerName;
        WelcomeText.text = "Welcome, " + PlayerName;
    }
    
}
