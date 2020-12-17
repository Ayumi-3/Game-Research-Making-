using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt; // player
    public Vector3 offset = new Vector3(0, 11.5f, -4.5f);
    public Vector3 rotation = new Vector3(41, 0, 0);

    private Vector3 DefaultPosition = new Vector3(-0.15f, 2.6f, 4.0f);
    private Vector3 DefaultRotation = new Vector3(15.0f, -160.0f, 0.0f);

    public bool IsRunning { set; get; }
    
    private void Update()
    {
        if (!IsRunning)
            return;

        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.05f);
    }

    public void SetDefault()
    {
        transform.position = DefaultPosition;
        transform.rotation = Quaternion.Euler(DefaultRotation);
    }
}
