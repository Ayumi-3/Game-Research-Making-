using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMotor : MonoBehaviour
{
    public Transform PlayerTransform;

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.forward * PlayerTransform.position.z;
    }
}
