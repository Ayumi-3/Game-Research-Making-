using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideObjectSpawner : MonoBehaviour
{
    private const float DISTANCE_TO_RESPAWN = 6.0f;

    public float ScrollSpeed = -7.0f;
    public float totalLenght;
    public bool IsScrolling { set; get; }

    private float scrollLocation;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        IsScrolling = false;
    }

    private void Update()
    {
        if (!IsScrolling)
            return;

        scrollLocation += ScrollSpeed * Time.deltaTime;
        Vector3 newLocation = (playerTransform.position.z + scrollLocation) * Vector3.forward;
        transform.position = newLocation;

        if(transform.GetChild(0).transform.position.z < playerTransform.position.z - DISTANCE_TO_RESPAWN)
        {
            transform.GetChild(0).localPosition += Vector3.forward * totalLenght;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);

            transform.GetChild(0).localPosition += Vector3.forward * totalLenght;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);
        }
    }

    public void SetDefault()
    {
        //transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        /*foreach(Transform child in transform)
        {
            child.position += new Vector3(0.0f, 0.0f, scrollLocation);
        }*/
        
        IsScrolling = true;
        StartCoroutine(waitScrolling());

    }

    private IEnumerator waitScrolling()
    {
        yield return new WaitForSeconds(0.01f);
        IsScrolling = false;
    }

}
