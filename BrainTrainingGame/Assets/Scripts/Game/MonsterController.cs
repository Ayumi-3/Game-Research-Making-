using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public Transform lookAt; // player
    public Vector3 offset = new Vector3(0, 2.3f, 13.0f);
    public GameObject AroundObject;

    private bool isRunning = false;
    private float rand;
    public int colorFlag = 0;
    private int tempFlag;
    private bool isChangeColor = false;

    private void Start()
    {
        transform.position = lookAt.position + offset;

        rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
        colorFlag = (int)Mathf.Ceil(rand);
        AroundObject.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[colorFlag - 1];
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (!isChangeColor)
        {
            StartCoroutine(waitForColorChange());
        }
        
    }

    private IEnumerator waitForColorChange()
    {
        isChangeColor = true;
        
        tempFlag = colorFlag;
        do
        {
            rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
            colorFlag = (int)Mathf.Ceil(rand);
        } while (tempFlag == colorFlag);

        AroundObject.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[colorFlag - 1];
        yield return new WaitForSeconds(5.0f);
        isChangeColor = false;
    }

    public void StartRunning()
    {
        isRunning = true;
    }

    public void PauseRunning()
    {
        isRunning = false;
    }

}
