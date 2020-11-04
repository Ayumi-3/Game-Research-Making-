using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasControl : MonoBehaviour
{
    public Canvas RegisterCanvas;

    private void Start()
    {
        RegisterCanvas.gameObject.SetActive(false);
    }

    
}
