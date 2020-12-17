using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Transform lookAt; // player
    public Vector3 offset = new Vector3(0.0f, 5.0f, 6.0f);

    private void Update()
    {
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
    }


}
