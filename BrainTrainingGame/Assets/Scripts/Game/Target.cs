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
            GameControl.Instance.CannotGetTarget();
            Destroy(gameObject, 0.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            audioSource.Play();
            //anim.SetTrigger("Collected");
            //motor.PauseRunning();
            rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
            colorFlag = (int)Mathf.Ceil(rand);
            gameObject.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[colorFlag - 1];
            GameControl.Instance.GetTarget(colorFlag, true);
            StartCoroutine(waitForAttack());
        }
    }

    private IEnumerator waitForAttack()
    {
        yield return new WaitForSeconds(0.5f);
        //GameControl.Instance.ExistTarget();
        GameControl.Instance.GetTarget(colorFlag, false);
        Destroy(gameObject, 0.5f);
        spawner.TargetCount--;
    }
    
}
