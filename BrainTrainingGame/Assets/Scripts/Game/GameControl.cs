using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    private const int TARGET_SCORE_AMOUNT = 10;
    private const int MISS_TARGET_SCORE_AMOUNT = -5;
    private const int MONSTER_HP_DECREASE = -20;
    private const int MONSTER_HP_INCREASE = 10;
    private const int OBSTACLE_HIT = -10;

    public static GameControl Instance { set; get; }

    private bool isGameStarted = false;
    private PlayerMotor player;
    private TargetSpawner targetSpawner;
    private MonsterMotor monsterMotor;
    private GameSetting gameSetting;

    public Canvas ScoreCanvas;
    public Canvas MonsterHPCanvas;
    public Canvas GameSettingCanvas;
    public Canvas CountDownCanvas;
    public Text CountDownText;

    // UI and UI fields
    public Text scoreText;
    private float score = 0.0f, targetScore = 0.0f;
    private int lastScore = 0;
    private int monsterColorFlag = 0;
    private int targetColorFlag = 0;
    private bool targetIsAttackable = false;

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

    private void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        targetSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetSpawner>();
        monsterMotor = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterMotor>();
        gameSetting = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameSetting>();
        scoreText.text = "Score: " + score.ToString("0");
        playerBillboard.text = "";
        monsterBillboard.text = "";
        CountDownText.text = "";

        ScoreCanvas.gameObject.SetActive(false);
        MonsterHPCanvas.gameObject.SetActive(false);
        GameSettingCanvas.gameObject.SetActive(true);
        CountDownCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(isFinishSetting && !isGameStarted)
        {
            isGameStarted = true;
            player.StartRunning();
            targetSpawner.StartRunning();
            monsterMotor.StartRunning();
            isFinishSetting = false;
        }
        
        if(isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) && targetIsAttackable)
            {
                targetIsAttackable = false;
                if (monsterColorFlag != targetColorFlag)
                {
                    updateScore(TARGET_SCORE_AMOUNT);
                    updateMonsterHP(MONSTER_HP_DECREASE);
                }
                else
                {
                    updateScore(-TARGET_SCORE_AMOUNT);
                    updateMonsterHP(MONSTER_HP_INCREASE);
                }

                if (monsterHP <= 0) //game stop
                {
                    isGameStarted = false;
                }
            }

            if (!isGameStarted)
            {
                player.PauseRunning();
                targetSpawner.PauseRunning();
                monsterMotor.PauseRunning();
                //Game ending
                GameSettingCanvas.gameObject.SetActive(false);
            }
        }
        
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
        yield return new WaitForSeconds(0.5f);
        billboard.text = "";
    }

    public void StartOnClick()
    {
        settingData = gameSetting.GetSetting();

        maxMonsterHP = int.Parse(settingData["MonsterMaxHp"]);
        monsterHP = maxMonsterHP;
        monsterHealthBar.SetMaxHealth(maxMonsterHP);

        player.speed = float.Parse(settingData["PlayerSpeed"]);
        ColorsPicker.Instance.colorMaxNumber = int.Parse(settingData["NumberOfColor"]);
        targetSpawner.obstacleChance = float.Parse(settingData["ObstacleAppearance"]) / 100.0f;
        targetSpawner.targetDistance = float.Parse(settingData["TargetDistance"]);
        isAdaptive = (settingData["AdaptiveToggle"] == "true");
        if (isAdaptive)
        {
            thresholdPoint = float.Parse(settingData["ThresholdPoint"]);
        }
        isConectedToGtec = (settingData["ConnectToGtecToggle"] == "true");
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

    public void GetTarget(int colorFlag, bool isAttackable)
    {
        monsterColorFlag = monsterMotor.colorFlag;
        targetColorFlag = colorFlag;

        targetIsAttackable = isAttackable;
    }

    public void ExistTarget()
    {
        if (isGameStarted)
        {
            player.StartRunning();
        }
    }

    public void CannotGetTarget()
    {
        updateScore(MISS_TARGET_SCORE_AMOUNT);
    }
    
    public void GetObstacle()
    {
        updateScore(OBSTACLE_HIT);
    }
}
