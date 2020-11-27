using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public InputField TimeInput;
    public InputField MonsterMaxHpInput;
    public InputField PlayerSpeedInput;
    public InputField NumberOfColorInput;
    public InputField ObstacleAppearanceInput;
    public InputField TargetDistanceInput;
    public Toggle AdaptiveToggle;
    public InputField ThresholdPointInput;
    public Toggle ConnectToGtecToggle;

    private Dictionary<string, string> DefaultData = new Dictionary<string, string>();
    private Dictionary<string, string> SettingData = new Dictionary<string, string>();

    private void Start()
    {
        DefaultData["Time"] = "300";
        DefaultData["MonsterMaxHp"] = "300";
        DefaultData["PlayerSpeed"] = "7";
        DefaultData["NumberOfColor"] = "3";
        DefaultData["ObstacleAppearance"] = "20";
        DefaultData["TargetDistance"] = "6";
        DefaultData["AdaptiveToggle"] = "false";
        DefaultData["ThresholdPoint"] = "80";
        DefaultData["ConnectToGtecToggle"] = "false";

        TimeInput.text = DefaultData["Time"];
        MonsterMaxHpInput.text = DefaultData["MonsterMaxHp"];
        PlayerSpeedInput.text = DefaultData["PlayerSpeed"];
        NumberOfColorInput.text = DefaultData["NumberOfColor"];
        ObstacleAppearanceInput.text = DefaultData["ObstacleAppearance"];
        TargetDistanceInput.text = DefaultData["TargetDistance"];
        AdaptiveToggle.isOn = (DefaultData["AdaptiveToggle"] == "true");
        ThresholdPointInput.text = "80";
        ConnectToGtecToggle.isOn = (DefaultData["ConnectToGtecToggle"] == "true");
    }

    public void SetDefault()
    {
        TimeInput.text = DefaultData["Time"];
        MonsterMaxHpInput.text = DefaultData["MonsterMaxHp"];
        PlayerSpeedInput.text = DefaultData["PlayerSpeed"];
        NumberOfColorInput.text = DefaultData["NumberOfColor"];
        ObstacleAppearanceInput.text = DefaultData["ObstacleAppearance"];
        TargetDistanceInput.text = DefaultData["TargetDistance"];
        AdaptiveToggle.isOn = (DefaultData["AdaptiveToggle"] == "true");
        ThresholdPointInput.text = "80";
        ConnectToGtecToggle.isOn = (DefaultData["ConnectToGtecToggle"] == "true");
    }

    public Dictionary<string, string> GetSetting()
    {
        SettingData["Time"] = TimeInput.text;
        SettingData["MonsterMaxHp"] = MonsterMaxHpInput.text;
        SettingData["PlayerSpeed"] = PlayerSpeedInput.text;
        SettingData["NumberOfColor"] = NumberOfColorInput.text;
        SettingData["ObstacleAppearance"] = ObstacleAppearanceInput.text;
        SettingData["TargetDistance"] = TargetDistanceInput.text;
        SettingData["AdaptiveToggle"] = AdaptiveToggle.isOn.ToString().ToLower();
        SettingData["ThresholdPoint"] = ThresholdPointInput.text;
        SettingData["ConnectToGtecToggle"] = ConnectToGtecToggle.isOn.ToString().ToLower();
        return SettingData;
    }

}
