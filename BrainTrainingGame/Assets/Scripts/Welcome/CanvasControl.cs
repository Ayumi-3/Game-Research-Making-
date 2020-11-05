using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour
{
    public static CanvasControl Instance { set; get; }

    public Canvas RegisterCanvas;
    //public Canvas MsgBoxCanvas;
    public string PlayerName = "Guest";
    public Text WelcomeText;
    public DataManager dataManager;
    public LoadSceneOnClick loadScene;

    private string dir = @"Data/";
    private string dataPath;
    private string csvName = "currentPlayerName.csv";
    private Dictionary<string, string> player = new Dictionary<string, string>();

    private void Awake()
    {
        Instance = this;
        RegisterCanvas.gameObject.SetActive(false);
        //MsgBoxCanvas.gameObject.SetActive(false);
        WelcomeText.text = "Welcome, " + PlayerName;
    }

    public void SetPlayerName(string playerName)
    {
        PlayerName = playerName;
        WelcomeText.text = "Welcome, " + PlayerName;
    }
    
    public void EnterMainMenu()
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        dataPath = dir + PlayerName + "/";
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        csvName = dir + csvName;
        player.Add("Player", PlayerName);
        //save current player name
        dataManager.WriteData(dir, csvName, player, true, false);
        loadScene.LoadByIndex(1); //go to MainMenu
        
    }
}
