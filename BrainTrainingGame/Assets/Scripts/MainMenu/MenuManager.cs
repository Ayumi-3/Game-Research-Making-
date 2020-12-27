using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { set; get; }
    
    public GetPlayerName getPlayerName;
    public Text WelcomeText;
    public string DataDir;

    private string dataPath = @"Data/";
    private string playerName;
    private DataManager dataManager;
    private AudioSource audioSource;
    private CommunicationController communicationController;
    private UDPReceiver udpReceiver;
    private LoadSceneOnClick loadSceneOnClick;
    private PlayerController playerController;

    // Baseline
    public Canvas BaselineCanvas;
    public Canvas BaselineSettingCanvas;
    public Text BaselineText;
    public Dropdown Mode; // 0= eyes open, 1= eyes closed
    public InputField TimeInput;
    private string baselineFile;
    private Dictionary<string, string> baselineData = new Dictionary<string, string>();
    private float timeRemain;
    private bool isRecording;
    private int recordMode;

    public Canvas QuestionnaireCanvas;

    private void Awake()
    {
        Instance = this;

        dataManager = GetComponent<DataManager>();
        audioSource = GetComponent<AudioSource>();
        communicationController = GetComponent<CommunicationController>();
        udpReceiver = GetComponent<UDPReceiver>();
        loadSceneOnClick = GetComponent<LoadSceneOnClick>();

        BaselineCanvas.gameObject.SetActive(false);
        QuestionnaireCanvas.gameObject.SetActive(false);

        playerName = getPlayerName.GetPlayer();

        WelcomeText.text = "Welcome, " + playerName;
        DataDir = dataPath + playerName + "/";
        if (!Directory.Exists(DataDir))
        {
            Directory.CreateDirectory(DataDir);
        }
        Debug.Log("Finish start MenuManager");
        
        TimeInput.text = "300";
        isRecording = false;
    }

    private void Update()
    {
        if (isRecording)
        {
            timeRemain -= Time.deltaTime;
            if (timeRemain <= 0.0f)
            {
                audioSource.Play();
                isRecording = false;
                baselineData["GtecEndTime"] = communicationController.ReceivedData.ToString();
                baselineData["UnityEndTime"] = System.DateTime.Now.ToString("HH-mm-ss.fff");
                dataManager.WriteData(DataDir, baselineFile, baselineData, true, true);
                BaselineText.text = "Finish";
                StartCoroutine(showFinishText());
            }
        }
    }

    private IEnumerator showFinishText()
    {
        yield return new WaitForSeconds(0.5f);
        BaselineSettingCanvas.gameObject.SetActive(true);
    }

    public void BaselineRecord()
    {
        baselineFile = DataDir + "Baseline_";
        if (Mode.value == 0)
        {
            baselineFile += "EyesOpen_";
        }
        else
        {
            baselineFile += "EyesClosed_";
        }

        baselineFile += System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        timeRemain = float.Parse(TimeInput.text);
        baselineData["SetTime"] = TimeInput.text;
        
        BaselineSettingCanvas.gameObject.SetActive(false);

        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        for (int i = 3; i > 0; i--)
        {
            BaselineText.text = i.ToString("0");
            yield return new WaitForSeconds(1.0f);
        }

        BaselineText.text = "Ready";
        yield return new WaitForSeconds(1.0f);

        BaselineText.text = "START";
        yield return new WaitForSeconds(1.0f);

        BaselineText.text = "+";

        baselineData["GtecStartTime"] = communicationController.ReceivedData.ToString();
        baselineData["UnityStartTime"] = System.DateTime.Now.ToString("HH-mm-ss.fff");
        isRecording = true;
    }

    public void StartGameScene()
    {
        udpReceiver.ChangeScene();
        loadSceneOnClick.LoadByIndex(2);
    }



}
