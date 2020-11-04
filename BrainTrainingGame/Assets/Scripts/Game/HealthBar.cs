using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text text;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        text.text = slider.value.ToString("0") + "/" + slider.maxValue.ToString("0");
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        text.text = slider.value.ToString("0") + "/" + slider.maxValue.ToString("0");
    }


}
