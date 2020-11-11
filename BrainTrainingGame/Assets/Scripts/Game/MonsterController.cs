using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public Transform lookAt; // player
    public Vector3 offset = new Vector3(0, 0, 8.0f);
    public GameObject AroundObject;

    private Vector3 DefaultPosition = new Vector3(0.0f, 0.0f, 8.0f);

    private Animator anim;

    private bool isRunning = false;
    private float rand;
    public int colorFlag = 0;
    private int tempFlag;
    private bool isChangeColor = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Damage", false);

        transform.position = lookAt.position + offset;

        rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
        colorFlag = (int)Mathf.Ceil(rand);
        AroundObject.GetComponent<Renderer>().material.color = ColorsPicker.Instance.Colors[colorFlag - 1];
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
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
    public void Damage()
    {
        anim.SetBool("Damage", true);
        StartCoroutine(WaitDamageAnimation());
    }

    private IEnumerator WaitDamageAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Damage", false);
    }
    
    public void SetDefault()
    {
        transform.position = DefaultPosition;
    }
}
