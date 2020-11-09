using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefault : MonoBehaviour
{
    private GameSetting gameSetting;

    private void Start()
    {
        gameSetting = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameSetting>();
    }

    public void OnClick()
    {
        gameSetting.SetDefault();
    }
}
