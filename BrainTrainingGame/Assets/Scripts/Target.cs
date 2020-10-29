﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Animator anim;
    private GameObject Player;
    private PlayerMotor motor;
    private TargetSpawner spawner;
    private int colorFlag = 1;
    private Color32 objectMat;
    private float rand;
    private float distanceZ;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        motor = Player.GetComponent<PlayerMotor>();
        spawner = Player.GetComponent<TargetSpawner>();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        distanceZ = Player.transform.position.z - gameObject.transform.position.z;
        if (distanceZ > 6)
        {
            spawner.TargetCount--;
            GameControl.Instance.CannotGetTarget();
            Destroy(gameObject, 0.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //anim.SetTrigger("Collected");
            motor.PauseRunning();
            rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
            colorFlag = (int)Mathf.Ceil(rand);
            gameObject.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[colorFlag - 1];
            GameControl.Instance.GetTarget(colorFlag, true);
            StartCoroutine(waitForAttack());
            

        }
    }

    private IEnumerator waitForAttack()
    {
        yield return new WaitForSeconds(1.0f);
        GameControl.Instance.ExistTarget();
        GameControl.Instance.GetTarget(colorFlag, false);
        Destroy(gameObject, 0.5f);
        spawner.TargetCount--;
    }
    
}
