using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using ViveSR.anipal.Eye;

public class GameControl : MonoBehaviour
{
    private const int RESPONSE_TO_RIGHT_SCORE_AMOUNT = 10;
    private const int AVOID_RESPONSE_TO_WRONG_SCORE_AMOUNT = 5;
    private const int LATE_RESPONSE_SCORE_AMOUNT = -5;
    private const int GET_TARGET_SCORE_AMOUNT = 5;
    private const int AVOID_OBSTACLE_SCORE_AMOUNT = 5;
    private const int MISS_TARGET_SCORE_AMOUNT = -1;
    private const int HIT_OBSTACLE_SCORE_AMOUNT = -5;
    private const int MONSTER_HP_DECREASE = -20;
    private const int MONSTER_HP_INCREASE = 10;
    private const int DEFEATED_MONSTER_SCORE_AMOUNT = 100;

    private const int MODE_CDT = 0;
    private const int MODE_TTT = 1;
    private const int MODE_MULTITASKING = 2;

    public static GameControl Instance { set; get; }

    private bool isGameStarted = false;
    private PlayerController playerController;
    private TargetSpawner targetSpawner;
    private MonsterSpawner monsterSpawner;
    private MonsterController monsterController;
    private Renderer monsterJewelRenderer;
    private GameSetting gameSetting;
    private SideObjectSpawner objectSpawner;
    private CameraMotor cameraMotor;
    private DataManager dataManager;
    private GetPlayerName getPlayerName;
    private CommunicationController communicationController;
    private UDPReceiver udpReceiver;
    private LoadSceneOnClick loadSceneOnClick;
    private Camera mainCamera;

    public Canvas ScoreCanvas;
    public Canvas MonsterHPCanvas;
    public Canvas GameSettingCanvas;
    public Canvas CountDownCanvas;
    public Text CountDownText;
    public Canvas GameEndCanvas;
    public Text GameEndScoreText;
    public Canvas GameEndMonitorCanvas;
    public Text GameEndMonitorScoreText;

    // UI and UI fields
    public Text scoreText;
    public Text defeatedText;
    public Text timeLeftText;
    private float score = 0.0f;
    private int monsterColorFlag = 0;
    private int targetColorFlag = 0;
    private bool targetIsAttackable = false;
    public GameObject PlayerClothColor;
    public GameObject PlayerHatColor;
    private int sessionNo;

    public HealthBar monsterHealthBar;
    public int maxMonsterHP = 300;
    private int monsterHP = 0;
    private int monsterId = 0;

    public Text playerBillboard;
    private Text monsterBillboard;

    private Dictionary<string, string> settingData = new Dictionary<string, string>();
    private bool isFinishSetting = false;
    private int gameMode;
    
    private float adaptiveTimer;
    private float timeInterval = 60.0f;
    private float responseTimeWindow = 0.5f;
    
    private float allCDTPoint;
    private float scoredCDTPoint;
    private float allTTTPoint;
    private float scoredTTTPoint;

    private AudioSource audioSource;

    private string dataPath = @"Data/";
    private string playerName;
    private string dataDir;
    private string csvName;
    public Dictionary<string, string> GamePlayData = new Dictionary<string, string>();
    private string eyeTrackingFile;
    private Dictionary<string, string> eyeTrackingData = new Dictionary<string, string>();
    private float[] eyeOpenness = new float[3];
    private VerboseData verboseData;

    private float maxTime;
    private float timeRemain;
    private bool isTimePause;

    //adaptive, level adjustment
    private List<float> CDTLevel = new List<float>();
    private float lvl1TimeWindow = 1000.0f;
    private float stepTimeWindow = 20.0f;
    private int numberCDTLevel = 38;
    private float CDTThresholdAccuracy = 80.0f;
    private float stepCDTAccuracy = 4.0f;
    private int CDTLimitChangeLevel = 4;
    private float CDTIntervalTime;
    private float minCDTIntervalTime = 2.0f;
    private float maxCDTIntervalTime = 3.5f;
    private float CDTTimer;

    private List<float> TTTSpeedLevel = new List<float>();
    private List<float> TTTDistanceLevel = new List<float>();
    private float minSpeed = 3.0f;
    private float maxSpeed = 30.0f;
    private float stepSpeed = 0.5f;
    private float minTimeInterval = 0.8f;
    private float maxTimeInterval = 2.0f;
    private float TTTThresholdAccuracy = 80.0f;
    private float stepTTTAccuracy = 2.0f;
    private int TTTLimitChangeLevel = 10;
    private float TTTFixedDistance = 10.0f;

    private int currentCDTLevel = 25;
    private int currentTTTLevel = 14;
    
    private void Awake()
    {
        Instance = this;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        targetSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetSpawner>();
        monsterSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<MonsterSpawner>();
        gameSetting = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameSetting>();
        objectSpawner = GameObject.FindGameObjectWithTag("SideObject").GetComponent<SideObjectSpawner>();
        cameraMotor = FindObjectOfType<CameraMotor>();
        audioSource = GetComponent<AudioSource>();
        dataManager = GetComponent<DataManager>();
        getPlayerName = GetComponent<GetPlayerName>();
        communicationController = GetComponent<CommunicationController>();
        udpReceiver = GetComponent<UDPReceiver>();
        loadSceneOnClick = GetComponent<LoadSceneOnClick>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();

        clearVariables();

        ScoreCanvas.gameObject.SetActive(false);
        MonsterHPCanvas.gameObject.SetActive(false);
        GameSettingCanvas.gameObject.SetActive(true);
        CountDownCanvas.gameObject.SetActive(false);
        GameEndMonitorCanvas.gameObject.SetActive(false);
        GameEndCanvas.gameObject.SetActive(false);

        // Get player name, get folder path, create folder if not exist
        playerName = getPlayerName.GetPlayer();
        dataDir = dataPath + playerName + "/";
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        PlayerClothColor.GetComponent<Renderer>().material.color = Color.white;
        PlayerHatColor.GetComponent<Renderer>().material.color = Color.white;

        sessionNo = 0;

        //setting task level
        settingTaskLevel();
    }
    
    private void Update()
    {
        if(isFinishSetting && !isGameStarted)
        {
            isGameStarted = true;
            playerController.StartRunning(gameMode);
            objectSpawner.IsScrolling = true;
            if (gameMode == MODE_TTT || gameMode == MODE_MULTITASKING)
            {
                targetSpawner.StartRunning();
            }
            if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
            {
                monsterController.StartRunning();
            }
            isFinishSetting = false;
        }
        
        if(isGameStarted)
        {
            RecordEyeTrackingData(false);

            adaptiveTimer += Time.deltaTime;
            if (adaptiveTimer > timeInterval)
            {
                adaptiveTimer = 0.0f;

                if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
                {
                    CDTLevelAdjustment();
                }
                if (gameMode == MODE_TTT || gameMode == MODE_MULTITASKING)
                {
                    TTTLevelAdjustment();
                }
            }

            if ((SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.Space)))
            {
                GameDataRecord(false, "Attack", "0", "0", "0", "0", "0",
                    "0", "0", "0", targetIsAttackable.ToString(), "0", "0",
                    "0", "0", "0", "0", "0", "0",
                    "0", "0", "0", "0", "0", "0", "0");

                if (targetIsAttackable && !isTimePause)
                {
                    targetIsAttackable = false;
                    if (monsterColorFlag != targetColorFlag) // Corectly response to right color
                    {
                        playerController.Attack(true);
                        monsterController.Damage();
                        updateScore(RESPONSE_TO_RIGHT_SCORE_AMOUNT);
                        updateMonsterHP(MONSTER_HP_DECREASE);
                        countColorDiscriminationTaskPoint(true);
                        GameDataRecord(false, "ResponseToRightColor", "0", "0", "0", "0", "0",
                            "0", "0", "1", "1", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                            "0", "0", "0", RESPONSE_TO_RIGHT_SCORE_AMOUNT.ToString(), monsterId.ToString(), ((float)monsterHP/(float)maxMonsterHP*100.0f).ToString(),
                            "0", "0", "0", "0", "0", "0", "0");
                    }
                    else // Response to wrong color
                    {
                        playerController.Attack(false);
                        updateScore(-RESPONSE_TO_RIGHT_SCORE_AMOUNT);
                        updateMonsterHP(MONSTER_HP_INCREASE);
                        countColorDiscriminationTaskPoint(false);
                        GameDataRecord(false, "ResponseToWrongColor", "0", "0", "0", "0", "0",
                            "0", "0", "-1", "1", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                            "0", "0", "0", (-RESPONSE_TO_RIGHT_SCORE_AMOUNT).ToString(), monsterId.ToString(), ((float)monsterHP / (float)maxMonsterHP * 100.0f).ToString(),
                            "0", "0", "0", "0", "0", "0", "0");
                    }

                    if (monsterHP <= 0) //monster is dead
                    {
                        defeatedText.text = monsterId.ToString("0");
                        isTimePause = true;
                        updateScore(DEFEATED_MONSTER_SCORE_AMOUNT);
                        MonsterHPCanvas.gameObject.SetActive(false);
                        monsterController.Dead();
                        GameDataRecord(false, "DefeatedMonster", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", DEFEATED_MONSTER_SCORE_AMOUNT.ToString(),
                            monsterId.ToString(), "0", "0", "0", "0", "0", "0", "0", "0");

                        playerController.PauseRunning();
                        objectSpawner.IsScrolling = false;
                        playerController.Ready();

                        StartCoroutine(waitDeadMonsterAnimation());
                    }
                }
            }

            if (gameMode == MODE_CDT || gameMode ==MODE_MULTITASKING)
            {
                //CDT control
                CDTTimer += Time.deltaTime;
                if (CDTTimer > CDTIntervalTime)
                {
                    CDTTimer = 0.0f;
                    CDTIntervalTime = Random.Range(minCDTIntervalTime, maxCDTIntervalTime);
                    PlayerChangeColor();
                }
            }
            
            if (!isTimePause)
            {
                timeRemain -= Time.deltaTime;
                if (timeRemain > 0.0f)
                {
                    float min, sec;
                    min = Mathf.Floor(timeRemain / 60f);
                    sec = timeRemain - (min * 60f);
                    timeLeftText.text = min.ToString() + ":" + sec.ToString("00.000");
                }
                else //game stop
                {
                    timeLeftText.text = "0:00.000";
                    GameDataRecord(false, "TimesUp", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                        "0", "0", "0", "0", "0", "0", "0", "0");
                    isGameStarted = false;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Manually stop game");
                GameDataRecord(false, "ManuallyStopGame", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                    "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                isGameStarted = false;
            }

            if (!isGameStarted)
            {
                playerController.PauseRunning();
                objectSpawner.IsScrolling = false;
                if (gameMode == MODE_TTT || gameMode == MODE_MULTITASKING)
                {
                    targetSpawner.PauseRunning();
                }
                if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
                {
                    monsterController.PauseRunning();
                }
                cameraMotor.IsRunning = false;
                if (monsterHP > 0)
                {
                    //targetIsAttackable = false;
                    monsterId--;
                }
                GameDataRecord(false, "TotalScore", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                    monsterId.ToString(), "0", "0", "0", "0", "0", "0", "0", "0");
                GameEndScoreText.text = "Your Score: " + score.ToString("0");
                if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
                {
                    GameEndScoreText.text += "\nDefeated Monsters: " + monsterId.ToString("0");
                }
                GameEndCanvas.gameObject.SetActive(true);
                GameEndMonitorScoreText.text = "Sessions: " + sessionNo.ToString("0") +
                    "\nScore: " + score.ToString("0");
                if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
                {
                    GameEndMonitorScoreText.text += "\nDefeated Monsters: " + monsterId.ToString("0") + "\nCDT Level: " + currentCDTLevel.ToString("0");
                }
                if (gameMode == MODE_TTT || gameMode == MODE_MULTITASKING)
                {
                    GameEndMonitorScoreText.text += "\nTTT Level: " + currentTTTLevel.ToString("0");
                }
                GameEndMonitorCanvas.gameObject.SetActive(true);
            }
        }
        
    }

    private IEnumerator waitDeadMonsterAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        prepareMonster();
        MonsterHPCanvas.gameObject.SetActive(true);
        playerController.StartRunning(gameMode);
        objectSpawner.IsScrolling = true;
        monsterController.StartRunning();
        isTimePause = false;
    }

    private void clearVariables()
    {
        scoreText.text = score.ToString("0");
        playerBillboard.text = "";
        CountDownText.text = "";

        score = 0.0f;
        adaptiveTimer = 0.0f;
        allCDTPoint = 0.0f;
        scoredCDTPoint = 0.0f;
        allTTTPoint = 0.0f;
        scoredTTTPoint = 0.0f;

        CDTTimer = 0.0f;

        monsterId = 0;
        defeatedText.text = monsterId.ToString("0");

        settingData.Clear();
    }

    public void StartOnClick()
    {
        settingData = gameSetting.GetSetting();

        gameMode = int.Parse(settingData["Mode"]);

        maxTime = float.Parse(settingData["Time"]);
        timeRemain = maxTime;

        maxMonsterHP = int.Parse(settingData["MonsterMaxHp"]);

        currentCDTLevel = int.Parse(settingData["CDTLevel"]);
        currentTTTLevel = int.Parse(settingData["TTTLevel"]);
        
        targetSpawner.obstacleChance = float.Parse(settingData["ObstacleAppearance"]) / 100.0f;

        responseTimeWindow = CDTLevel[currentCDTLevel] / 1000.0f;

        playerController.speed = TTTSpeedLevel[currentTTTLevel];
        objectSpawner.ScrollSpeed = -playerController.speed;
        targetSpawner.targetDistance = TTTFixedDistance;

        //playerController.speed = 30;
        //objectSpawner.ScrollSpeed = -playerController.speed;
        //targetSpawner.targetDistance = 10;

        sessionNo++;
        // set start record data
        string startTime = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        csvName = dataDir + "GameDataRecord_" + startTime + "_" + sessionNo.ToString("00") + ".csv";
        dataManager.WriteData(dataDir, csvName, settingData, true, true);
        GameDataRecord(true, "StartGame", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
            "0", currentCDTLevel.ToString(), responseTimeWindow.ToString(), "0", currentTTTLevel.ToString(), playerController.speed.ToString(), targetSpawner.targetDistance.ToString());
        //communicationController.SendTriggerToMatlab(true);

        eyeTrackingFile = dataDir + "EyeTrackingRecord_" + startTime + "_" + sessionNo.ToString("00") + ".csv";
        RecordEyeTrackingData(true);

        if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
        {
            // Prepare Monster
            prepareMonster();

            CDTIntervalTime = Random.Range(minCDTIntervalTime, maxCDTIntervalTime);
        }

        timeLeftText.text = "0:00.000";

        cameraMotor.IsRunning = true;
        playerController.Ready();
        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        GameSettingCanvas.gameObject.SetActive(false);
        CountDownCanvas.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            CountDownText.text = i.ToString("0");
            yield return new WaitForSeconds(1.0f);
        }

        CountDownText.text = "Ready";
        yield return new WaitForSeconds(1.0f);

        CountDownText.text = "START";
        yield return new WaitForSeconds(1.0f);
        
        CountDownCanvas.gameObject.SetActive(false);
        ScoreCanvas.gameObject.SetActive(true);
        if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
        {
            MonsterHPCanvas.gameObject.SetActive(true);
        }
        isFinishSetting = true;
        isTimePause = false;
    }

    private void RecordEyeTrackingData(bool isFirst)
    {
        eyeTrackingData["GtecTime"] = communicationController.ReceivedData.ToString();
        eyeTrackingData["UnityTime"] = (Time.time * 1000).ToString();  //System.DateTime.Now.ToString("HH-mm-ss.fff");
        EyeData_v2 eyeData = new EyeData_v2();
        SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
        SRanipal_Eye_v2.GetVerboseData(out verboseData);
        eyeTrackingData["CombinedEyeValidDataBitMask"] = verboseData.combined.eye_data.eye_data_validata_bit_mask.ToString();
        eyeTrackingData["LeftEyeValidDataBitMask"] = verboseData.left.eye_data_validata_bit_mask.ToString();
        eyeTrackingData["RightEyeValidDataBitMask"] = verboseData.right.eye_data_validata_bit_mask.ToString();
        eyeTrackingData["CombinedEyeOpenness"] = verboseData.combined.eye_data.eye_openness.ToString();
        eyeTrackingData["LeftEyeOpenness"] = verboseData.left.eye_openness.ToString();
        eyeTrackingData["RightEyeOpenness"] = verboseData.right.eye_openness.ToString();
        eyeTrackingData["CombinedPupilDiameter"] = verboseData.combined.eye_data.pupil_diameter_mm.ToString();
        eyeTrackingData["LeftPupilDiameter"] = verboseData.left.pupil_diameter_mm.ToString();
        eyeTrackingData["RightPupilDiameter"] = verboseData.right.pupil_diameter_mm.ToString();
        eyeTrackingData["CombinedPupilPosition"] = verboseData.combined.eye_data.pupil_position_in_sensor_area.ToString();
        eyeTrackingData["LeftPupilPosition"] = verboseData.left.pupil_position_in_sensor_area.ToString();
        eyeTrackingData["RightPupilPosition"] = verboseData.right.pupil_position_in_sensor_area.ToString();
        eyeTrackingData["CombinedGazeOrigin"] = verboseData.combined.eye_data.gaze_origin_mm.ToString();
        eyeTrackingData["LeftGazeOrigin"] = verboseData.left.gaze_origin_mm.ToString();
        eyeTrackingData["RightGazeOrigin"] = verboseData.right.gaze_origin_mm.ToString();
        eyeTrackingData["CombinedGazeDirection"] = verboseData.combined.eye_data.gaze_direction_normalized.ToString();
        eyeTrackingData["LeftGazeDirection"] = verboseData.left.gaze_direction_normalized.ToString();
        eyeTrackingData["RightGazeDirection"] = verboseData.right.gaze_direction_normalized.ToString();

        eyeTrackingData["LeftEyeWide"] = eyeData.expression_data.left.eye_wide.ToString();
        eyeTrackingData["RightEyeWide"] = eyeData.expression_data.right.eye_wide.ToString();
        eyeTrackingData["LeftEyeSqeeze"] = eyeData.expression_data.left.eye_squeeze.ToString();
        eyeTrackingData["RightEyeSqeeze"] = eyeData.expression_data.right.eye_squeeze.ToString();
        eyeTrackingData["LeftEyeFrown"] = eyeData.expression_data.left.eye_frown.ToString();
        eyeTrackingData["RightEyeFrown"] = eyeData.expression_data.right.eye_frown.ToString();

        Ray ray;
        FocusInfo focusInfo;
        bool focusStatus = SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out ray, out focusInfo);
        //Debug.Log("FocusStatus: " + focusStatus);
        eyeTrackingData["FocusStatus"] = focusStatus.ToString();
        if (focusStatus)
        {
            Debug.Log(focusInfo.collider);
            eyeTrackingData["RayOriginal"] = ray.origin.ToString();
            eyeTrackingData["RayDirection"] = ray.direction.ToString();
            eyeTrackingData["Collider"] = focusInfo.collider.ToString();
            eyeTrackingData["Distance"] = focusInfo.distance.ToString();
            eyeTrackingData["Normal"] = focusInfo.normal.ToString();
            eyeTrackingData["Point"] = focusInfo.point.ToString();
            eyeTrackingData["Transform.position"] = focusInfo.transform.position.ToString();
            eyeTrackingData["Transform.rotation"] = focusInfo.transform.rotation.ToString();
        }
        else
        {
            eyeTrackingData["RayOriginal"] = "0";
            eyeTrackingData["RayDirection"] = "0";
            eyeTrackingData["Collider"] = "0";
            eyeTrackingData["Distance"] = "0";
            eyeTrackingData["Normal"] = "0";
            eyeTrackingData["Point"] = "0";
            eyeTrackingData["Transform.position"] = "0";
            eyeTrackingData["Transform.rotation"] = "0";
        }

        eyeTrackingData["Camera.position"] = mainCamera.transform.position.ToString();
        eyeTrackingData["Camera.rotation"] = mainCamera.transform.rotation.ToString();

        dataManager.WriteData(dataDir, eyeTrackingFile, eyeTrackingData, isFirst, isFirst);
    }

    private void prepareMonster()
    {
        monsterId++;
        monsterSpawner.SpawnMonster();
        monsterController = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>();
        monsterBillboard = GameObject.FindGameObjectWithTag("Monster").GetComponentInChildren<Text>();
        monsterJewelRenderer = monsterController.JewelRenderer;
        monsterBillboard.text = "";
        monsterHP = maxMonsterHP;
        monsterHealthBar.SetMaxHealth(maxMonsterHP);
        GameDataRecord(false, "SpawnedMonster", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"
            , monsterId.ToString(), "0", "0", "0", "0", "0", "0", "0", "0");
    }

    private void updateScore(int addScore)
    {
        score += addScore;
        scoreText.text = score.ToString("0");
        StartCoroutine(showBillboard(playerBillboard, addScore));
    }

    private void updateMonsterHP(int addMonsterHP)
    {
        monsterHP += addMonsterHP;
        monsterHP = (int)Mathf.Clamp(monsterHP, 0, maxMonsterHP);
        monsterHealthBar.SetHealth(monsterHP);
        StartCoroutine(showBillboard(monsterBillboard, addMonsterHP));
    }

    private IEnumerator showBillboard(Text billboard, int value)
    {
        if (value >= 0)
        {
            billboard.color = Color.green;
        }
        else
        {
            billboard.color = Color.red;
        }
        billboard.text = value.ToString("0");
        yield return new WaitForSeconds(0.3f);
        billboard.text = "";
    }

    private void countColorDiscriminationTaskPoint(bool isScored)
    {
        allCDTPoint += 1.0f;
        if (isScored)
        {
            scoredCDTPoint += 1.0f;
        }
    }

    private void countTargetTrackingTaskPoint(bool isScored)
    {
        allTTTPoint += 1.0f;
        if (isScored)
        {
            scoredTTTPoint += 1.0f;
        }
    }

    private void settingTaskLevel()
    {
        //Color Discrimination Task
        int i = 0;
        CDTLevel.Add(lvl1TimeWindow);
        for (i = 1; i < numberCDTLevel; i++)
        {
            CDTLevel.Add(CDTLevel[i - 1] - stepTimeWindow);
        }

        //Target Tracking Task
        int j = 0;
        TTTSpeedLevel.Add(minSpeed);
        for (j = 1; TTTSpeedLevel[j - 1] < maxSpeed; j++)
        {
            TTTSpeedLevel.Add(TTTSpeedLevel[j - 1] + stepSpeed);
        }
        
        //float stepTimeInterval, tempDistance;
        //stepTimeInterval = (maxTimeInterval - minTimeInterval) / TTTSpeedLevel.Count;
        //j = 0;
        //TTTDistanceLevel.Add(TTTSpeedLevel[j] * maxTimeInterval);
        //for (j = 1; j < TTTSpeedLevel.Count; j++)
        //{
        //    tempDistance = TTTSpeedLevel[j] * (maxTimeInterval - (stepTimeInterval * j));
        //    tempDistance *= 10.0f;
        //    tempDistance = Mathf.Floor(tempDistance);
        //    tempDistance /= 10.0f;
        //    TTTDistanceLevel.Add(tempDistance);
        //}
    }

    private void CDTLevelAdjustment() //Color Discrimination Task level adjustment
    {
        float currentAccuracy, accuracyDifferent;
        int adjustLevel = 0;
        currentAccuracy = scoredCDTPoint / allCDTPoint * 100.0f;
        scoredCDTPoint = 0.0f;
        allCDTPoint = 0.0f;
        accuracyDifferent = currentAccuracy - CDTThresholdAccuracy;
        if (Mathf.Abs(accuracyDifferent) > stepCDTAccuracy)
        {
            adjustLevel = (int)Mathf.Floor(accuracyDifferent / stepCDTAccuracy);
            adjustLevel = Mathf.Clamp(adjustLevel, -CDTLimitChangeLevel, CDTLimitChangeLevel);

            currentCDTLevel += adjustLevel;
            currentCDTLevel = Mathf.Clamp(currentCDTLevel, 0, CDTLevel.Count - 1);

            responseTimeWindow = CDTLevel[currentCDTLevel] / 1000.0f;
        }
        GameDataRecord(false, "CDTLevelAdjustment", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", "0", "0",
            "0","0", "0", "0", "0", "0",
            currentAccuracy.ToString(), currentCDTLevel.ToString(), responseTimeWindow.ToString(), "0", "0", "0", "0");
        Debug.Log("CDT:" + currentAccuracy + "|" + adjustLevel + "|" + currentCDTLevel + "|" + responseTimeWindow);
    }

    private void TTTLevelAdjustment() //Target Tracking Task level adjustment
    {
        float currentAccuracy, accuracyDifferent;
        int adjustLevel = 0;
        currentAccuracy = scoredTTTPoint / allTTTPoint * 100.0f;
        scoredTTTPoint = 0.0f;
        allTTTPoint = 0.0f;
        accuracyDifferent = currentAccuracy - TTTThresholdAccuracy;
        if (Mathf.Abs(accuracyDifferent) > stepTTTAccuracy)
        {
            adjustLevel = (int)Mathf.Floor(accuracyDifferent / stepTTTAccuracy);
            adjustLevel = Mathf.Clamp(adjustLevel, -TTTLimitChangeLevel, TTTLimitChangeLevel);

            currentTTTLevel += adjustLevel;
            currentTTTLevel = Mathf.Clamp(currentTTTLevel, 0, TTTSpeedLevel.Count - 1);

            playerController.speed = TTTSpeedLevel[currentTTTLevel];
            objectSpawner.ScrollSpeed = -playerController.speed;
            //targetSpawner.targetDistance = TTTDistanceLevel[currentTTTLevel];
        }
        GameDataRecord(false, "TTTLevelAdjustment", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", "0", "0",
            "0", "0", "0", currentAccuracy.ToString(), currentTTTLevel.ToString(), playerController.speed.ToString(), targetSpawner.targetDistance.ToString());
        Debug.Log("TTT:" + currentAccuracy + "|" + adjustLevel + "|" + currentTTTLevel + "|" + playerController.speed + "|" + targetSpawner.targetDistance);
    }

    public void PlayerChangeColor()
    {
        float rand;
        rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
        targetColorFlag = (int)Mathf.Ceil(rand);

        //monsterColorFlag = monsterController.colorFlag;

        PlayerClothColor.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[targetColorFlag - 1];
        PlayerHatColor.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[targetColorFlag - 1];
        
        rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
        monsterColorFlag = (int)Mathf.Ceil(rand);

        monsterJewelRenderer.materials[1].color = ColorsPicker.Instance.Colors[monsterColorFlag - 1];

        GameDataRecord(false, "ChangePlayerColor", "0", "0", "0", "0", "0",
            "0", "0", "1", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
            "0", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", "0", "0", "0");

        targetIsAttackable = true;
        StartCoroutine(waitForAttack());
    }

    public void GetTarget() // Reach the target
    {
        updateScore(GET_TARGET_SCORE_AMOUNT);
        countTargetTrackingTaskPoint(true);

        GameDataRecord(false, "GetTarget", "0", "0", "0", "0", "0",
            "1", "0", "0", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
            "0", "0", "0", GET_TARGET_SCORE_AMOUNT.ToString(), "0", "0",
            "0", "0", "0", "0", "0", "0", "0");
    }

    private IEnumerator waitForAttack()
    {
        yield return new WaitForSeconds(responseTimeWindow); // wait for response time

        PlayerClothColor.GetComponent<Renderer>().material.color = Color.white;
        PlayerHatColor.GetComponent<Renderer>().material.color = Color.white;
        monsterJewelRenderer.materials[1].color = Color.white;

        if (targetIsAttackable)
        {
            if (monsterColorFlag != targetColorFlag) // Late response to right color
            {
                updateScore(LATE_RESPONSE_SCORE_AMOUNT);
                countColorDiscriminationTaskPoint(false);
                GameDataRecord(false, "LateResponse", "0", "0", "0", "0", "0",
                  "0", "0", "1", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                  "0", "0", "0", LATE_RESPONSE_SCORE_AMOUNT.ToString(), "0", "0",
                  "0", "0", "0", "0", "0", "0", "0");
            }
            else // correctly avoid response to wrong color
            {
                updateScore(AVOID_RESPONSE_TO_WRONG_SCORE_AMOUNT);
                countColorDiscriminationTaskPoint(true);
                GameDataRecord(false, "AvoidResponse", "0", "0", "0", "0", "0",
                    "0", "0", "-1", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                    "0", "0", "0", AVOID_RESPONSE_TO_WRONG_SCORE_AMOUNT.ToString(), "0", "0",
                    "0", "0", "0", "0", "0", "0", "0");
            }
        }
        targetIsAttackable = false;
    }

    public void CannotGetTarget(Transform player, Transform target) // Miss the target
    {
        updateScore(MISS_TARGET_SCORE_AMOUNT);
        countTargetTrackingTaskPoint(false);
        GameDataRecord(false, "MissTarget", "0", "0", "0", "0", player.position.x.ToString(),
            "-1", target.position.x.ToString(), "0", "0", "0", "0",
            "0", "0", "0", MISS_TARGET_SCORE_AMOUNT.ToString(), "0", "0",
            "0", "0", "0", "0", "0", "0", "0");
    }
    
    public void GetObstacle() // Hit obstacle
    {
        updateScore(HIT_OBSTACLE_SCORE_AMOUNT);
        countTargetTrackingTaskPoint(false);
        GameDataRecord(false, "HitObstacle", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", "0", "0",
            "-1", "0", "0", HIT_OBSTACLE_SCORE_AMOUNT.ToString(), "0", "0",
            "0", "0", "0", "0", "0", "0", "0");

        playerController.Fall();
    }

    public void DidntGetObatacle(Transform player, Transform obstacle) // Avoid obstacle
    {
        updateScore(AVOID_OBSTACLE_SCORE_AMOUNT);
        countTargetTrackingTaskPoint(true);
        GameDataRecord(false, "AvoidObstacle", "0", "0", "0", "0", player.position.x.ToString(),
            "0", "0", "0", "0", "0", "0",
            "1", obstacle.position.x.ToString(), "0", AVOID_OBSTACLE_SCORE_AMOUNT.ToString(), "0", "0",
            "0", "0", "0", "0", "0", "0", "0");
    }

    public void MonsterChangeColor()
    {
        GameDataRecord(false, "MonsterChangeColor", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", monsterController.colorFlag.ToString(), targetColorFlag.ToString(),
            "0", "0", "1", "0", "0", "0",
            "0", "0", "0", "0", "0", "0", "0");
    }

    public void PlayAgain()
    {
        playerController.SetDefault();
        cameraMotor.SetDefault();
        if (gameMode == MODE_TTT || gameMode == MODE_MULTITASKING)
        {
            targetSpawner.ClearTarget();
        }
        if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
        {
            monsterController.DestroyMonster();
        }
        objectSpawner.SetDefault();

        gameSetting.SetLevelNextSession(currentCDTLevel, currentTTTLevel);

        clearVariables();
        ScoreCanvas.gameObject.SetActive(false);
        MonsterHPCanvas.gameObject.SetActive(false);
        GameSettingCanvas.gameObject.SetActive(true);
        CountDownCanvas.gameObject.SetActive(false);
        GameEndCanvas.gameObject.SetActive(false);
        GameEndMonitorCanvas.gameObject.SetActive(false);

        //audioSource.Play();
    }

    public void GameDataRecord(bool isFirst, string gameEvent, string moveLeft, string moveRight, string buttonDown, string buttonUp,
        string playerPosition, string getTarget, string targetPosition, string attackable, string attack, string monsterColorId,
        string targetColorId, string avoidObstacle, string obstaclePosition, string monsterColorChange, string getScore,
        string monsterId, string monsterHpPercent, string cdtAccuracy, string cdtLevel, string timeWindow,
        string tttAccuracy, string tttLevel, string speed, string distance)
    {
        GamePlayData["GtecTime"] = communicationController.ReceivedData.ToString();
        GamePlayData["UnityTime"] = (Time.time * 1000).ToString(); //System.DateTime.Now.ToString("HH-mm-ss.fff");
        GamePlayData["GameEvent"] = gameEvent;
        GamePlayData["MoveLeft"] = moveLeft;
        GamePlayData["MoveRight"] = moveRight;
        GamePlayData["ButtonDown"] = buttonDown;
        GamePlayData["ButtonUp"] = buttonUp;
        GamePlayData["PlayerPosition"] = playerPosition;
        GamePlayData["GetTarget"] = getTarget;
        GamePlayData["TargetPosition"] = targetPosition;
        GamePlayData["Attackable"] = attackable;
        GamePlayData["Attack"] = attack;
        GamePlayData["MonsterColorId"] = monsterColorId;
        GamePlayData["TargetColorId"] = targetColorId;
        GamePlayData["AvoidObstacle"] = avoidObstacle;
        GamePlayData["ObstaclePosition"] = obstaclePosition;
        GamePlayData["MonsterColorChange"] = monsterColorChange;
        GamePlayData["GetScore"] = getScore;
        GamePlayData["MonsterId"] = monsterId;
        GamePlayData["MonsterHpPercent"] = monsterHpPercent;
        GamePlayData["CDTAccuracy"] = cdtAccuracy;
        GamePlayData["CDTLevel"] = cdtLevel;
        GamePlayData["TimeWindow"] = timeWindow;
        GamePlayData["TTTAccuracy"] = tttAccuracy;
        GamePlayData["TTTLevel"] = tttLevel;
        GamePlayData["Speed"] = speed;
        GamePlayData["Distance"] = distance;
        GamePlayData["Score"] = score.ToString();

        dataManager.WriteData(dataDir, csvName, GamePlayData, false, isFirst);
    }
    
    public void BackToMainMenuScene()
    {
        udpReceiver.ChangeScene();
        loadSceneOnClick.LoadByIndex(1);
    }

}
