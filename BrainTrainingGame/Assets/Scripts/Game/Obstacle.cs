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

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        motor = Player.GetComponent<PlayerController>();
        spawner = Player.GetComponent<TargetSpawner>();
        //anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        distanceZ = Player.transform.position.z - gameObject.transform.position.z;
        if (distanceZ > 3)
        {
            spawner.TargetCount--;
            Destroy(gameObject, 0.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            audioSource.Play();
            //motor.PauseRunning();
            GameControl.Instance.GetObstacle();
            Destroy(gameObject, 1f);
            spawner.TargetCount--;
        }
    }
}
