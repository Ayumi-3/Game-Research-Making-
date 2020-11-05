using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public InputField MonsterMaxHpInput;
    public InputField PlayerSpeedInput;
    public InputField NumberOfColorInput;
    public InputField ObstacleAppearanceInput;
    public InputField TargetDistanceInput;
    public Toggle AdaptiveToggle;
    public InputField ThresholdPointInput;
    public Toggle ConnectToGtecToggle;

    private Dictionary<string, string> DefaultData = new Dictionary<string, string>();

    private void Start()
    {
        DefaultData["MonsterMaxHp"] = "300";
        DefaultData["PlayerSpeed"] = "7";
        DefaultData["NumberOfColor"] = "3";
        DefaultData["ObstacleAppearance"] = "20";
        DefaultData["TargetDistance"] = "6";
        DefaultData["AdaptiveToggle"] = "false";
        DefaultData["ThresholdPoint"] = "80";
        DefaultData["ConnectToGtecToggle"] = "false";
    }

    public void SetDefault()
    {
        MonsterMaxHpInput.text = DefaultData["MonsterMaxHp"];
        PlayerSpeedInput.text = DefaultData["PlayerSpeed"];
        NumberOfColorInput.text = DefaultData["NumberOfColor"];
        ObstacleAppearanceInput.text = DefaultData["ObstacleAppearance"];
        TargetDistanceInput.text = DefaultData["TargetDistance"];
        AdaptiveToggle.isOn = (DefaultData["AdaptiveToggle"] == "true");
        ThresholdPointInput.text = "80";
        ConnectToGtecToggle.isOn = (DefaultData["ConnectToGtecToggle"] == "true");
    }

}
