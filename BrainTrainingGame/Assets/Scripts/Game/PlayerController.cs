using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour
{
    private const float LANE_DISTANCE = 3.0f;
    private const float TURN_SPEED = 0.05f;

    private bool isRunning = false;

    // movement
    private CharacterController controller;
    public float speed = 7.0f;
    private int desiredLane = 3; // 5Lanes

    //Animator
    private Animator anim;

    private Transform monsterTransform;
    private SideObjectSpawner objectSpawner;

    public SteamVR_Action_Vector2 TouchAction;
    private Vector2 touchValue;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        monsterTransform = GameObject.FindGameObjectWithTag("Monster").transform;
        objectSpawner = GameObject.FindGameObjectWithTag("SideObject").GetComponent<SideObjectSpawner>();
        PauseRunning();
    }
    private void Update()
    {
        if (!isRunning)
            return;

        // Gather the inputs on which lane we should be
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    MoveLane(false);
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //    MoveLane(true);
        if (SteamVR_Actions._default.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            touchValue = TouchAction.GetAxis(SteamVR_Input_Sources.Any);

            if (touchValue.x < -0.2f)
            {
                MoveLane(false);
            }
            if (touchValue.x > 0.2f)
            {
                MoveLane(true);
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
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;
        moveVector.y = 0.0f;
        moveVector.z = speed;

        // Move the Player
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

    public void StartRunning()
    {
        isRunning = true;
        anim.SetBool("Walk", true);
        anim.SetBool("Rest", false);
        anim.SetBool("Attack", false);

        objectSpawner.IsScrolling = true;
    }

    public void PauseRunning()
    {
        isRunning = false;
        anim.SetBool("Walk", false);
        anim.SetBool("Rest", true);
        anim.SetBool("Attack", false);

        objectSpawner.IsScrolling = false;
    }

    public void Attack()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Rest", false);
        anim.SetBool("Attack", true);

        transform.LookAt(monsterTransform.position);
    }

    public void SetDefault()
    {
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
    }
}
