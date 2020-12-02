using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    private const float LANE_DISTANCE = 3.0f;
    private const float DISTANCE_TO_RESPAWN = 30.0f;

    public GameObject target;
    public GameObject obstacle;
    public Transform player;
    public int maxTarget = 5;
    public float targetDistance = 6.0f;
    public float obstacleChance = 0.2f;

    private bool isRunning = false;
    private int randomLane = 3, tempLane;
    public int TargetCount = 0;
    private Transform targetPosition;
    private Vector3 currentPosition;
    private float obstacleRand;
    private int obstacleNumber = 0;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        TargetCount = 0;
    }

    private void Update()
    {
        if (!isRunning)
            return;
        
        if (TargetCount == 0)
        {
            currentPosition = new Vector3(target.transform.position.x, target.transform.position.y, player.position.z);
            tempLane = (int)Mathf.Ceil(Random.Range(0.0f, 5.0f));
            laneRandom();
            currentPosition.z += 5;
            Instantiate(target, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.identity);
            TargetCount++;
        }
        else if (TargetCount < maxTarget) //currentPosition.z < player.position.z + (targetDistance * 3)
        {
            currentPosition = new Vector3(0.0f, 0.0f, currentPosition.z);
            tempLane = randomLane;
            laneRandom();

            if (obstacleNumber < 2)
            {
                obstacleRand = Random.Range(0.0f, 1.0f);
                if (obstacleRand >= obstacleChance)
                {
                    Instantiate(target, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.identity);
                    obstacleNumber = 0;
                }
                else
                {
                    Instantiate(obstacle, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                    obstacleNumber++;
                }
            }
            else
            {
                Instantiate(target, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z), Quaternion.identity);
                obstacleNumber = 0;
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

    public void ClearTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach(GameObject obj in targets)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in obstacles)
        {
            Destroy(obj);
        }

        TargetCount = 0;
        
    }
}
