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
    private PlayerMotor motor;
    private TargetSpawner targetSpawner;
    private MonsterMotor monsterMotor;

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

    private void Awake()
    {
        Instance = this;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        targetSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetSpawner>();
        monsterMotor = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterMotor>();
        scoreText.text = "Score: " + score.ToString("0");
        playerBillboard.text = "";
        monsterBillboard.text = "";
        monsterHP = maxMonsterHP;
        monsterHealthBar.SetMaxHealth(maxMonsterHP);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartRunning();
            targetSpawner.StartRunning();
            monsterMotor.StartRunning();
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
                motor.PauseRunning();
                targetSpawner.PauseRunning();
                monsterMotor.PauseRunning();
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
            motor.StartRunning();
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
