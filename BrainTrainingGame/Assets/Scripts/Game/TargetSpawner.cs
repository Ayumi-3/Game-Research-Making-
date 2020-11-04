using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    private const float LANE_DISTANCE = 3.0f;

    //private PlayerMotor motor;

    public GameObject target;
    public GameObject obstacle;
    public Transform player;
    public int maxTarget = 10;
    public float targetDistance = 6.0f;
    public float obstacleChance = 0.2f;

    private bool isRunning = false;
    private int randomLane = 3, tempLane;
    public int TargetCount = 0;
    private Transform targetPosition;
    private Vector3 currentPosition;
    private float obstacleRand;


    private void Update()
    {
        if (!isRunning)
            return;
        
        
        if (TargetCount == 0)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            currentPosition = new Vector3(target.transform.position.x, target.transform.position.y, player.position.z);
            tempLane = (int)Mathf.Ceil(Random.Range(0.0f, 5.0f));
            laneRandom();
            Instantiate(target, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.identity);
            TargetCount++;
        }
        else if (TargetCount < maxTarget)
        {
            currentPosition = new Vector3(0.0f, 0.0f, currentPosition.z);
            tempLane = randomLane;
            laneRandom();

            obstacleRand = Random.Range(0.0f, 1.0f);
            if (obstacleRand >= obstacleChance)
            {
                Instantiate(target, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.identity);
            }
            else
            {
                Instantiate(obstacle, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.identity);
            }
            
            TargetCount++;
        }
        
    }

    private void laneRandom()
    {
        do
        {
            randomLane = (int)Mathf.Ceil(Random.Range(0.0f, 5.0f));
        } while ((tempLane == randomLane) || (Mathf.Abs(randomLane - tempLane) > 1));
        currentPosition += new Vector3(0.0f, 0.0f, targetDistance);

        switch(randomLane)
        {
            case 1:
                currentPosition += Vector3.left * LANE_DISTANCE * 2;
                break;
            case 2:
                currentPosition += Vector3.left * LANE_DISTANCE;
                break;
            case 3: //default
                break;
            case 4:
                currentPosition += Vector3.right * LANE_DISTANCE;
                break;
            case 5:
                currentPosition += Vector3.right * LANE_DISTANCE * 2;
                break;
        }

    }

    public void StartRunning()
    {
        isRunning = true;
    }

    public void PauseRunning()
    {
        isRunning = false;
    }
}
