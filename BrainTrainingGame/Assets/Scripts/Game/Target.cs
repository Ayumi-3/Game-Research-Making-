using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Animator anim;
    private GameObject Player;
    private PlayerController motor;
    private TargetSpawner spawner;
    private int colorFlag = 1;
    private Color32 objectMat;
    private float rand;
    private float distanceZ;
    private AudioSource audioSource;
    private bool isGetTarget = false;

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
            if (!isGetTarget)
            {
                GameControl.Instance.CannotGetTarget(Player.transform, gameObject.transform);
            }
            Destroy(gameObject, 0.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            audioSource.Play();
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            GameControl.Instance.GetTarget();
            isGetTarget = true;
        }
    }
    
}
