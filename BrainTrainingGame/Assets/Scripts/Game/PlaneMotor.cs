using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMotor : MonoBehaviour
{
    private Transform PlayerTransform;
    public Vector3 offset = new Vector3(0.0f, 0.0f, 60.0f);

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.forward * PlayerTransform.position.z + offset;
    }
}
