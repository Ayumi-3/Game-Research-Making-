using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Questionnaire : MonoBehaviour
{
    public static Questionnaire Instance { set; get; }

    public GameObject[] QuestionPanel;
    public Button PreviousButton;
    public Button NextButton;
    public Button FinishButton;

    public Dropdown Q1_1dropdown;
    public InputField Q1_2inputfield;
    public Dropdown Q1_3dropdown;
    public Dropdown Q1_4dropdown;
    public Dropdown Q1_5dropdown;
    public Dropdown Q1_6dropdown;
    public InputField Q1_7inputfield;
    public Dropdown Q2_1dropdown;
    public InputField Q2_1inputfield;
    public Dropdown Q2_21dropdown;
    public Dropdown Q2_22dropdown;
    public Dropdown Q2_23dropdown;
    public Dropdown Q2_31dropdown;
    public Dropdown Q2_32dropdown;
    public Dropdown Q2_33dropdown;
    public Dropdown Q2_4dropdown;
    public InputField Q2_5inputfield;

    private Dictionary<string, string> answer = new Dictionary<string, string>();
    private DataManager dataManager;
    private GetPlayerName getPlayerName;
    private string playerName;
    private string dataDir = @"Data/";
    private string csvName = "Questionnaire.csv";

    private int QP;

    public void Start()
    {
        Instance = this;

        dataManager = GetComponent<DataManager>();
        getPlayerName = GetComponent<GetPlayerName>();

        QuestionPanel[0].gameObject.SetActive(true);
        QuestionPanel[1].gameObject.SetActive(false);
        QuestionPanel[2].gameObject.SetActive(false);
        PreviousButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        FinishButton.gameObject.SetActive(false);
        QP = 0;
    }
    
    private void manageQuestionPanel()
    {
        for (int i = 0; i < 3; i++)
        {
            if (QP == i)
            {
                QuestionPanel[i].gameObject.SetActive(true);
            }
            else
            {
                QuestionPanel[i].gameObject.SetActive(false);
            }
        }

        if (QP >= 1)
        {
            PreviousButton.gameObject.SetActive(true);
        }
        else
        {
            PreviousButton.gameObject.SetActive(false);
        }
        if (QP <= 1)
        {
            NextButton.gameObject.SetActive(true);
            FinishButton.gameObject.SetActive(false);
        }
        else
        {
            NextButton.gameObject.SetActive(false);
            FinishButton.gameObject.SetActive(true);
        }
    }

    public void NextQuestionPanel()
    {
        QP++;
        manageQuestionPanel();
    }

    public void PreviousQuestionPanel()
    {
        QP--;
        manageQuestionPanel();
    }

    public void SubmitQuestionnaire()
    {
        playerName = getPlayerName.GetPlayer();

        answer["Subject"] = playerName;
        answer["Q1_1"] = Q1_1dropdown.value.ToString() + " " + Q1_1dropdown.options[Q1_1dropdown.value].text;
        answer["Q1_2"] = Q1_2inputfield.text;
        answer["Q1_3"] = Q1_3dropdown.value.ToString() + " " + Q1_3dropdown.options[Q1_3dropdown.value].text;
        answer["Q1_4"] = Q1_4dropdown.value.ToString() + " " + Q1_4dropdown.options[Q1_4dropdown.value].text;
        answer["Q1_5"] = Q1_5dropdown.value.ToString() + " " + Q1_5dropdown.options[Q1_5dropdown.value].text;
        answer["Q1_6"] = Q1_6dropdown.value.ToString() + " " + Q1_6dropdown.options[Q1_6dropdown.value].text;
        answer["Q1_7"] = Q1_7inputfield.text;

        answer["Q2_1"] = Q2_1dropdown.value.ToString() + " " + Q2_1dropdown.options[Q2_1dropdown.value].text;
        answer["Q2_1Other"] = Q2_1inputfield.text;
        answer["Q2_21"] = Q2_21dropdown.value.ToString() + " " + Q2_21dropdown.options[Q2_21dropdown.value].text;
        answer["Q2_22"] = Q2_22dropdown.value.ToString() + " " + Q2_22dropdown.options[Q2_22dropdown.value].text;
        answer["Q2_23"] = Q2_23dropdown.value.ToString() + " " + Q2_23dropdown.options[Q2_23dropdown.value].text;
        answer["Q2_31"] = Q2_31dropdown.value.ToString() + " " + Q2_31dropdown.options[Q2_31dropdown.value].text;
        answer["Q2_32"] = Q2_32dropdown.value.ToString() + " " + Q2_32dropdown.options[Q2_32dropdown.value].text;
        answer["Q2_33"] = Q2_33dropdown.value.ToString() + " " + Q2_33dropdown.options[Q2_33dropdown.value].text;
        answer["Q2_4"] = Q2_4dropdown.value.ToString() + " " + Q2_4dropdown.options[Q2_4dropdown.value].text;
        answer["Q2_5"] = Q2_5inputfield.text;
        
        dataManager.WriteData(dataDir, dataDir + csvName, answer, false, false);
    }
}
