using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour
{
    private const float LANE_DISTANCE = 3.0f;
    private const float TURN_SPEED = 0.05f;

    private const int MODE_CDT = 0;
    private const int MODE_TTT = 1;
    private const int MODE_MULTITASKING = 2;

    private bool isRunning = false;
    private bool isReady = false;

    private int gameMode = 0;

    // movement
    private CharacterController controller;
    public float speed = 7.0f;
    private int desiredLane = 3; // 5Lanes

    //Animator
    private Animator anim;
    private float randAction;

    private Transform monsterTransform;

    public SteamVR_Action_Vector2 TouchAction;
    private Vector2 touchValue;

    private AudioSource audioSource;
    public AudioClip GetPointSound;
    public AudioClip NotGetPointSound;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        PauseRunning();
        isReady = false;
        transform.position = Vector3.zero;

    }
    private void Update()
    {
        if (!isRunning)
        {
            if (!isReady)
            {
                randAction = Random.Range(0.0f, 1.0f);
                if (randAction < 0.3f)
                {
                    anim.SetTrigger("Idle1");
                }
                else if (randAction < 0.6f)
                {
                    anim.SetTrigger("Idle2");
                }
            }
            return;
        }

        if (gameMode == MODE_TTT || gameMode == MODE_MULTITASKING)
        {
            // Gather the inputs on which lane we should be
            // Keyboard
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLane(false);
                GameControl.Instance.GameDataRecord(false, "MoveLeftKeyDown", "1", "0", "1", "0", transform.position.x.ToString(),
                    "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveLane(true);
                GameControl.Instance.GameDataRecord(false, "MoveRightKeyDown", "0", "1", "1", "0", transform.position.x.ToString(),
                    "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                GameControl.Instance.GameDataRecord(false, "MoveLeftKeyUp", "1", "0", "0", "1", transform.position.x.ToString(),
                    "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                GameControl.Instance.GameDataRecord(false, "MoveRightKeyUp", "0", "1", "0", "1", transform.position.x.ToString(),
                    "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
            }
            // VR
            if (SteamVR_Actions._default.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
            {
                touchValue = TouchAction.GetAxis(SteamVR_Input_Sources.Any);

                if (touchValue.x < -0.2f)
                {
                    MoveLane(false);
                    GameControl.Instance.GameDataRecord(false, "MoveLeftKeyDown", "1", "0", "1", "0", transform.position.x.ToString(),
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                }
                if (touchValue.x > 0.2f)
                {
                    MoveLane(true);
                    GameControl.Instance.GameDataRecord(false, "MoveRightKeyDown", "0", "1", "1", "0", transform.position.x.ToString(),
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                }
            }
            if (SteamVR_Actions._default.Teleport.GetStateUp(SteamVR_Input_Sources.Any))
            {
                touchValue = TouchAction.GetAxis(SteamVR_Input_Sources.Any);

                if (touchValue.x < -0.2f)
                {
                    GameControl.Instance.GameDataRecord(false, "MoveLeftKeyUp", "1", "0", "0", "1", transform.position.x.ToString(),
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                }
                if (touchValue.x > 0.2f)
                {
                    GameControl.Instance.GameDataRecord(false, "MoveRightKeyUp", "0", "1", "0", "1", transform.position.x.ToString(),
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                }
            }
        }

        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 1)
            targetPosition += Vector3.left * LANE_DISTANCE * 2;
        else if (desiredLane == 2)
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if (desiredLane == 4)
            targetPosition += Vector3.right * LANE_DISTANCE;
        else if (desiredLane == 5)
            targetPosition += Vector3.right * LANE_DISTANCE * 2;
        
        // Let's calculate our move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).x * speed;
        moveVector.y = 0.0f;
        moveVector.z = speed;
        
        //// Move the Player
        controller.Move(moveVector * Time.deltaTime);
        
        // Rotate the player
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 1, 5);
    }
    
    public void StartRunning(int mode)
    {
        isRunning = true;
        anim.SetBool("Walk", true);
        anim.SetBool("Rest", false);
        gameMode = mode;
        if (gameMode == MODE_CDT || gameMode == MODE_MULTITASKING)
        {
            monsterTransform = GameObject.FindGameObjectWithTag("Monster").transform;
        }
        
    }

    public void PauseRunning()
    {
        isRunning = false;
        anim.SetBool("Walk", false);
        anim.SetBool("Rest", true);
        isReady = false;
    }

    public void Ready()
    {
        isReady = true;
        anim.ResetTrigger("Idle1");
        anim.ResetTrigger("Idle2");
    }

    public void Attack(bool getPoint)
    {
        transform.LookAt(monsterTransform.position);

        anim.SetTrigger("Attack");
        if (getPoint)
        {
            audioSource.PlayOneShot(GetPointSound);
        }
        else
        {
            audioSource.PlayOneShot(NotGetPointSound);
        }
    }

    public void Fall()
    {
        anim.SetTrigger("Fall");
    }

    public void SetDefault()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        desiredLane = 3;
    }
}
