using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject Monster;
    public Transform Player;
    public Vector3 offset = new Vector3(0, 0, 8.0f);

    private Vector3 targetPosition;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    public void SpawnMonster()
    {
        targetPosition = Player.position + offset;
        Instantiate(Monster, targetPosition, Quaternion.Euler(0.0f, 180.0f, 0.0f));
    }

}
