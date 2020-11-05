using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetPlayerName : MonoBehaviour
{
    public DataManager dataManager;

    private string dataPath = @"Data/";
    private string csvName = "currentPlayerName.csv";
    private string csvPath;
    private List<string> playerNameList = new List<string>();
    
    public string GetPlayer()
    {
        string playerName;
        csvPath = dataPath + csvName;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        if (!File.Exists(csvPath))
        {
            playerName = "Guest";
        }
        else
        {
            playerNameList = dataManager.ReadPlayerName(csvPath);
            if (playerNameList[0] == "")
            {
                playerName = "Guest";
            }
            else
            {
                playerName = playerNameList[0];
            }
        }
        
        return playerName;

    }
}
