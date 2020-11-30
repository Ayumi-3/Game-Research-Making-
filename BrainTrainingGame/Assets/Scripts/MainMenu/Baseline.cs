using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baseline : MonoBehaviour
{
    public void BaselineStartOnClick()
    {
        MenuManager.Instance.BaselineRecord();
    }
}
