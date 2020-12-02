using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private GameObject Player;
    private PlayerController motor;
    private TargetSpawner spawner;
    private float distanceZ;
    private AudioSource audioSource;
    private bool isGetObstacle = false;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        motor = Player.GetComponent<PlayerController>();
        spawner = Player.GetComponent<TargetSpawner>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        distanceZ = Player.transform.position.z - gameObject.transform.position.z;
        if (distanceZ > 3)
        {
            spawner.TargetCount--;
            if (!isGetObstacle)
            {
                GameControl.Instance.DidntGetObatacle(Player.transform, gameObject.transform);
            }
            Destroy(gameObject, 0.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            audioSource.Play();
            GameControl.Instance.GetObstacle();
            isGetObstacle = true;
        }
    }
}
