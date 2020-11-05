using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    
    public InputField NameInput;
    
    //private CanvasControl canvasControl;
    //
    //private void Start()
    //{
    //    canvasControl = GameObject.FindGameObjectWithTag("CanvasControl").GetComponent<CanvasControl>();
    //}
    //
    public void SavePlayerName()
    {
        if (NameInput.text != "")
        {
            CanvasControl.Instance.SetPlayerName(NameInput.text);
        }
            
    }
}
