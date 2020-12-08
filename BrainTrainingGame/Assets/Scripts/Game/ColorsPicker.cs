using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsPicker : MonoBehaviour
{
    public static ColorsPicker Instance { set; get; }

    public float colorMaxNumber = 4.0f;
    public Color32[] Colors = new Color32[5];

    private void Awake()
    {
        Instance = this;
    }
}
