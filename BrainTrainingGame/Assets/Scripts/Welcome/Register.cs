using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    
    public InputField NameInput;

    private CanvasControl canvasControl;

    private string dir = @"Data/";
    private string dataPath;

    private void Start()
    {
        canvasControl = GameObject.FindGameObjectWithTag("CanvasControl").GetComponent<CanvasControl>();
    }

    public void SaveData()
    {
        if(NameInput.text != "")
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dataPath = dir + NameInput.text + "/";

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            canvasControl.SetPlayerName(NameInput.text);
            //else
            //{
            //    canvasControl.MsgBoxCanvas.gameObject.SetActive(true);
            //}
        }
    }
}
