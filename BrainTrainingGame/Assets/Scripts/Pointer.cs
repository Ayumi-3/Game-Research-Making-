using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public float DefaultLength = 5.0f;
    public GameObject Dot;
    public VRInputModule InputModule;

    private LineRenderer lineRenderer = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // use default or distance
        float targetLength = DefaultLength;

        // raycast
        RaycastHit hit = CreateRaycast(targetLength);

        // default
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // or based on hit
        if (hit.collider != null)
        {
            endPosition = hit.point;
        }

        // set position of dot
        Dot.transform.position = endPosition;

        // set linerenderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, DefaultLength);

        return hit;
    }
}
