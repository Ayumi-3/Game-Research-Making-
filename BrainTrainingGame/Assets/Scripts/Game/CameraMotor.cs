using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt; // player
    public Vector3 offset = new Vector3(0, 11.0f, -4.0f);
    public float tiltAngle = 45.0f;

    private float tlitAroundX;
    private Quaternion rotate;

    private void Start()
    {
        transform.position = lookAt.position + offset;
        tlitAroundX = Input.GetAxis("Vertical") * tiltAngle;
        rotate = Quaternion.Euler(tlitAroundX, 0.0f, 0.0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime);
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
        /*tlitAroundX = Input.GetAxis("Vertical") * tiltAngle;
        rotate = Quaternion.Euler(tlitAroundX, 0.0f, 0.0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime);*/
    }
}
