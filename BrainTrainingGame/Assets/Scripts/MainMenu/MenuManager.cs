using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Canvas BaselineCanvas;
    public GetPlayerName getPlayerName;
    public Text WelcomeText;
    public string DataDir;

    private string dataPath = @"Data/";
    private string playerName;

    private void Start()
    {
        BaselineCanvas.gameObject.SetActive(false);

        playerName = getPlayerName.GetPlayer();

        WelcomeText.text = "Welcome, " + playerName;
        DataDir = dataPath + playerName + "/";
        if (!Directory.Exists(DataDir))
        {
            Directory.CreateDirectory(DataDir);
        }
        Debug.Log("Finish start MenuManager");
    }

    

}
