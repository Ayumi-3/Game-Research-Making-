using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public Dropdown ModeDropdown;
    public InputField TimeInput;
    public InputField MonsterMaxHpInput;
    public InputField CDTLevelInput;
    public InputField TTTLevelInput;
    public InputField ObstacleAppearanceInput;

    private Dictionary<string, string> DefaultData = new Dictionary<string, string>();
    private Dictionary<string, string> SettingData = new Dictionary<string, string>();

    private void Start()
    {
        DefaultData["Time"] = "300";
        DefaultData["MonsterMaxHp"] = "300";
        DefaultData["CDTLevel"] = "25";
        DefaultData["TTTLevel"] = "14";
        DefaultData["ObstacleAppearance"] = "20";
        
        TimeInput.text = DefaultData["Time"];
        MonsterMaxHpInput.text = DefaultData["MonsterMaxHp"];
        CDTLevelInput.text = DefaultData["CDTLevel"];
        TTTLevelInput.text = DefaultData["TTTLevel"];
        ObstacleAppearanceInput.text = DefaultData["ObstacleAppearance"];
    }

    public void SetDefault()
    {
        TimeInput.text = DefaultData["Time"];
        MonsterMaxHpInput.text = DefaultData["MonsterMaxHp"];
        CDTLevelInput.text = DefaultData["CDTLevel"];
        TTTLevelInput.text = DefaultData["TTTLevel"];
        ObstacleAppearanceInput.text = DefaultData["ObstacleAppearance"];
    }

    public Dictionary<string, string> GetSetting()
    {
        SettingData["Mode"] = ModeDropdown.value.ToString("0");
        SettingData["Time"] = TimeInput.text;
        SettingData["MonsterMaxHp"] = MonsterMaxHpInput.text;
        SettingData["CDTLevel"] = CDTLevelInput.text;
        SettingData["TTTLevel"] = TTTLevelInput.text;
        SettingData["ObstacleAppearance"] = ObstacleAppearanceInput.text;
        return SettingData;
    }

    public void SetLevelNextSession(int cdtlevel, int tttlevel)
    {
        CDTLevelInput.text = cdtlevel.ToString();
        TTTLevelInput.text = tttlevel.ToString();
    }

}
