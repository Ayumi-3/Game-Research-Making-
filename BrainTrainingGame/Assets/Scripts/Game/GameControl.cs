using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class GameControl : MonoBehaviour
{
    private const int TARGET_SCORE_AMOUNT = 10;
    private const int AVOID_RESPONSE_TO_WRONG_SCORE_AMOUNT = 5;
    private const int MISS_TARGET_SCORE_AMOUNT = -5;
    private const int OBSTACLE_HIT_SCORE_AMOUNT = -10;
    private const int MONSTER_HP_DECREASE = -20;
    private const int MONSTER_HP_INCREASE = 10;

    public static GameControl Instance { set; get; }

    private bool isGameStarted = false;
    private PlayerController player;
    private TargetSpawner targetSpawner;
    private MonsterController monsterController;
    private GameSetting gameSetting;
    private SideObjectSpawner objectSpawner;
    private CameraMotor cameraMotor;
    private DataManager dataManager;
    private GetPlayerName getPlayerName;
    private CommunicationController communicationController;

    public Canvas ScoreCanvas;
    public Canvas MonsterHPCanvas;
    public Canvas GameSettingCanvas;
    public Canvas CountDownCanvas;
    public Text CountDownText;
    public Canvas GameEndCanvas;
    public Text GameEndScoreText;

    // UI and UI fields
    public Text scoreText;
    private float score = 0.0f;
    private int monsterColorFlag = 0;
    private int targetColorFlag = 0;
    private bool targetIsAttackable = false;
    public GameObject PlayerSphere;

    public HealthBar monsterHealthBar;
    public int maxMonsterHP = 300;
    private int monsterHP = 0;

    public Text playerBillboard;
    public Text monsterBillboard;

    private Dictionary<string, string> settingData = new Dictionary<string, string>();
    private bool isAdaptive;
    private float thresholdPoint;
    private bool isConectedToGtec;
    private bool isFinishSetting = false;
    
    private float adaptiveTimer;
    private float timeInterval = 10.0f;
    private float pointRatio;
    private float playerSpeed;
    public float SpeedUp = 3.0f;

    private float allTarget;
    private float scoredTarget;
    private float allCDTPoint;
    private float scoreCDTPoint;
    private float allTNTPoint;
    private float scoredTNTPoint;

    private AudioSource audioSource;

    private string dataPath = @"Data/";
    private string playerName;
    private string dataDir;
    private string csvName;
    public Dictionary<string, string> GamePlayData = new Dictionary<string, string>();

    private void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        targetSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetSpawner>();
        monsterController = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>();
        gameSetting = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameSetting>();
        objectSpawner = GameObject.FindGameObjectWithTag("SideObject").GetComponent<SideObjectSpawner>();
        cameraMotor = FindObjectOfType<CameraMotor>();
        audioSource = GetComponent<AudioSource>();
        dataManager = GetComponent<DataManager>();
        getPlayerName = GetComponent<GetPlayerName>();
        communicationController = GetComponent<CommunicationController>();

        clearVariables();

        ScoreCanvas.gameObject.SetActive(false);
        MonsterHPCanvas.gameObject.SetActive(false);
        GameSettingCanvas.gameObject.SetActive(true);
        CountDownCanvas.gameObject.SetActive(false);
        GameEndCanvas.gameObject.SetActive(false);

        // Get player name, get folder path, create folder if not exist
        playerName = getPlayerName.GetPlayer();
        dataDir = dataPath + playerName + "/";
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
    }

    private void Update()
    {
        if(isFinishSetting && !isGameStarted)
        {
            isGameStarted = true;
            player.StartRunning();
            targetSpawner.StartRunning();
            monsterController.StartRunning();
            isFinishSetting = false;
        }
        
        if(isGameStarted)
        {
            if /*(SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) && targetIsAttackable)*/(Input.GetKeyDown(KeyCode.Space) && targetIsAttackable)
            {
                targetIsAttackable = false;
                if (monsterColorFlag != targetColorFlag) // Corectly response to right color
                {
                    player.Attack(true);
                    monsterController.Damage();
                    updateScore(TARGET_SCORE_AMOUNT);
                    updateMonsterHP(MONSTER_HP_DECREASE);
                    countColorDiscriminationTaskPoint(true);
                    GameDataRecord(false, "ResponseToRightColor", "0", "0", "0", "0", "0",
                        "0", "0", "1", "1", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                        "0", "0", "0", TARGET_SCORE_AMOUNT.ToString(), "0", ((float)monsterHP/(float)maxMonsterHP*100.0f).ToString());
                }
                else // Response to wrong color
                {
                    player.Attack(false);
                    updateScore(-TARGET_SCORE_AMOUNT);
                    updateMonsterHP(MONSTER_HP_INCREASE);
                    countColorDiscriminationTaskPoint(false);
                    GameDataRecord(false, "ResponseToWrongColor", "0", "0", "0", "0", "0",
                        "0", "0", "-1", "1", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                        "0", "0", "0", (-TARGET_SCORE_AMOUNT).ToString(), "0", ((float)monsterHP / (float)maxMonsterHP * 100.0f).ToString());
                }

                if (monsterHP <= 0) //game stop
                {
                    GameDataRecord(false, "DefeatedMonster", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                    isGameStarted = false;
                    audioSource.Pause();
                }
            }

            if (isAdaptive)
            {
                adaptiveTimer += Time.deltaTime;
                if (adaptiveTimer > timeInterval)
                {
                    adaptiveTimer = 0.0f;
                    pointRatio = scoredTarget / allTarget;
                    playerSpeed = player.speed;
                    if (pointRatio > thresholdPoint)
                    {
                        playerSpeed += SpeedUp;
                    }
                    else
                    {
                        playerSpeed -= 1.0f;
                    }
                    playerSpeed = Mathf.Clamp(playerSpeed, 1.0f, 20.0f);
                    player.speed = playerSpeed;
                    objectSpawner.ScrollSpeed = -player.speed;
                }
            }

            if (!isGameStarted)
            {
                player.PauseRunning();
                targetSpawner.PauseRunning();
                monsterController.PauseRunning();
                cameraMotor.IsRunning = false;
                GameEndScoreText.text = "Your Score: " + score.ToString("0");
                GameEndCanvas.gameObject.SetActive(true);
            }
        }
        
    }

    private void clearVariables()
    {
        scoreText.text = "Score: " + score.ToString("0");
        playerBillboard.text = "";
        monsterBillboard.text = "";
        CountDownText.text = "";

        score = 0.0f;
        allTarget = 0.0f;
        scoredTarget = 0.0f;
        adaptiveTimer = 0.0f;
        allCDTPoint = 0.0f;
        scoreCDTPoint = 0.0f;
        allTNTPoint = 0.0f;
        scoredTNTPoint = 0.0f;

        settingData.Clear();
    }

    public void StartOnClick()
    {
        settingData = gameSetting.GetSetting();

        maxMonsterHP = int.Parse(settingData["MonsterMaxHp"]);
        monsterHP = maxMonsterHP;
        monsterHealthBar.SetMaxHealth(maxMonsterHP);

        player.speed = float.Parse(settingData["PlayerSpeed"]);
        objectSpawner.ScrollSpeed = -player.speed;
        ColorsPicker.Instance.colorMaxNumber = int.Parse(settingData["NumberOfColor"]);
        targetSpawner.obstacleChance = float.Parse(settingData["ObstacleAppearance"]) / 100.0f;
        targetSpawner.targetDistance = float.Parse(settingData["TargetDistance"]);
        isAdaptive = (settingData["AdaptiveToggle"] == "true");
        if (isAdaptive)
        {
            thresholdPoint = float.Parse(settingData["ThresholdPoint"]) / 100.0f;
            Debug.Log("Adaptive threshold: " + thresholdPoint);
        }
        isConectedToGtec = (settingData["ConnectToGtecToggle"] == "true");

        csvName = dataDir + "GameDataRecord_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        dataManager.WriteData(dataDir, csvName, settingData, true, true);
        GameDataRecord(true, "StartGame", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
        //communicationController.SendTriggerToMatlab(true);

        cameraMotor.IsRunning = true;
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
        MonsterHPCanvas.gameObject.SetActive(true);
        isFinishSetting = true;
    }

    private void updateScore(int addScore)
    {
        score += addScore;
        scoreText.text = "Score: " + score.ToString("0");
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
            scoreCDTPoint += 1.0f;
        }
    }

    private void countTargetNavigationTaskPoint(bool isScored)
    {
        allTNTPoint += 1.0f;
        if (isScored)
        {
            scoredTNTPoint += 1.0f;
        }
    }

    private void countTarget(bool isScored)
    {
        allTarget += 1.0f;
        if (isScored)
        {
            scoredTarget += 1.0f;
        }
    }

    public void GetTarget(int colorFlag) // Reach the target
    {
        monsterColorFlag = monsterController.colorFlag;
        targetColorFlag = colorFlag;

        PlayerSphere.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[colorFlag - 1];
        countTargetNavigationTaskPoint(true);

        GameDataRecord(false, "GetTarget", "0", "0", "0", "0", "0",
           "1", "0", "0", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
           "0", "0", "0", "0", "0", "0");

        targetIsAttackable = true;
        StartCoroutine(waitForAttack());
    }

    private IEnumerator waitForAttack()
    {
        yield return new WaitForSeconds(0.5f); // wait for response time

        PlayerSphere.GetComponent<Renderer>().material.color = Color.white;

        if (targetIsAttackable)
        {
            if (monsterColorFlag != targetColorFlag) // Late response to right color
            {
                updateScore(-TARGET_SCORE_AMOUNT);
                countColorDiscriminationTaskPoint(false);
                GameDataRecord(false, "LateResponse", "0", "0", "0", "0", "0",
                  "0", "0", "1", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                  "0", "0", "0", (-TARGET_SCORE_AMOUNT).ToString(), "0", "0");
            }
            else // correctly avoid response to wrong color
            {
                updateScore(AVOID_RESPONSE_TO_WRONG_SCORE_AMOUNT);
                countColorDiscriminationTaskPoint(true);
                GameDataRecord(false, "AvoidResponse", "0", "0", "0", "0", "0",
                    "0", "0", "-1", "0", monsterColorFlag.ToString(), targetColorFlag.ToString(),
                    "0", "0", "0", AVOID_RESPONSE_TO_WRONG_SCORE_AMOUNT.ToString(), "0", "0");
            }
        }
        targetIsAttackable = false;
    }

    public void CannotGetTarget(Transform player, Transform target) // Miss the target
    {
        updateScore(MISS_TARGET_SCORE_AMOUNT);
        countTargetNavigationTaskPoint(false);
        GameDataRecord(false, "MissTarget", "0", "0", "0", "0", player.position.x.ToString(),
            "-1", target.position.x.ToString(), "0", "0", "0", "0",
            "0", "0", "0", MISS_TARGET_SCORE_AMOUNT.ToString(), "0", "0");
    }
    
    public void GetObstacle() // Hit obstacle
    {
        updateScore(OBSTACLE_HIT_SCORE_AMOUNT);
        countTargetNavigationTaskPoint(false);
        GameDataRecord(false, "HitObstacle", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", "0", "0",
            "-1", "0", "0", OBSTACLE_HIT_SCORE_AMOUNT.ToString(), "0", "0");

        player.Fall();
    }

    public void DidntGetObatacle(Transform player, Transform obstacle) // Avoid obstacle
    {
        countTargetNavigationTaskPoint(true);
        GameDataRecord(false, "AvoidObstacle", "0", "0", "0", "0", player.position.x.ToString(),
            "0", "0", "0", "0", "0", "0",
            "1", obstacle.position.x.ToString(), "0", "0", "0", "0");
    }

    public void MonsterChangeColor()
    {
        GameDataRecord(false, "MonsterChangeColor", "0", "0", "0", "0", "0",
            "0", "0", "0", "0", monsterController.colorFlag.ToString(), "0",
            "0", "0", "1", "0", "0", "0");
    }

    public void PlayAgain()
    {
        targetSpawner.ClearTarget();
        cameraMotor.SetDefault();
        player.SetDefault();
        monsterController.SetDefault();
        objectSpawner.SetDefault();

        clearVariables();
        ScoreCanvas.gameObject.SetActive(false);
        MonsterHPCanvas.gameObject.SetActive(false);
        GameSettingCanvas.gameObject.SetActive(true);
        CountDownCanvas.gameObject.SetActive(false);
        GameEndCanvas.gameObject.SetActive(false);

        audioSource.Play();
    }

    public void GameDataRecord(bool isFirst, string gameEvent, string moveLeft, string moveRight, string buttonDown, string buttonUp,
        string playerPosition, string getTarget, string targetPosition, string attackable, string attack, string monsterColorId,
        string targetColorId, string avoidObstacle, string obstaclePosition, string monsterColorChange, string getScore,
        string monsterId, string monsterHpPercent)
    {
        GamePlayData["GtecTime"] = communicationController.ReceivedData.ToString();
        GamePlayData["UnityTime"] = System.DateTime.Now.ToString("HH-mm-ss.fff");
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
        GamePlayData["Score"] = score.ToString();

        dataManager.WriteData(dataDir, csvName, GamePlayData, false, isFirst);
    }
    
}
