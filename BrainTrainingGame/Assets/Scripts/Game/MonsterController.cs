using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private Transform lookAt; // player
    public Vector3 offset = new Vector3(0, 0, 10.0f);
    public Renderer JewelRenderer;
    public Renderer BodyRenderer;
    public Renderer HatRenderer;
    public Material[] MonsterMaterialNormal;
    public Material MonsterMaterialDamage;

    private Material[] jewelMaterials;

    private Vector3 DefaultPosition = new Vector3(0.0f, 0.0f, 10.0f);

    private Animator anim;

    private bool isRunning = false;
    private float rand;
    public int colorFlag = 0;
    private int tempFlag;
    private bool isChangeColor = false;
    private int matIndex;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Walk", false);

        lookAt = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        rand = Random.Range(0.0f, MonsterMaterialNormal.Length);
        matIndex = (int)Mathf.Ceil(rand);
        BodyRenderer.material = MonsterMaterialNormal[matIndex - 1];
        HatRenderer.material = MonsterMaterialNormal[matIndex - 1];

        jewelMaterials = JewelRenderer.materials;
        jewelMaterials[1].color = Color.white;

        /*rand = Random.Range(0.0f, ColorsPicker.Instance.colorMaxNumber);
        colorFlag = (int)Mathf.Ceil(rand);
        jewelMaterials = JewelRenderer.materials;
        jewelMaterials[1].color = ColorsPicker.Instance.Colors[colorFlag - 1];*/
    }
    
    private void Update()
    {
        if (!isRunning)
            return;

        /*Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.1f);*/

        transform.position = Vector3.forward * lookAt.position.z + offset;

        /*if (!isChangeColor)
        {
            StartCoroutine(waitForColorChange());
        }*/

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
        
        jewelMaterials[1].color = ColorsPicker.Instance.Colors[colorFlag - 1];

        GameControl.Instance.MonsterChangeColor();

        yield return new WaitForSeconds(3.0f);
        isChangeColor = false;
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetBool("Walk", true);
    }

    public void PauseRunning()
    {
        isRunning = false;
        anim.SetBool("Walk", false);
    }
    public void Damage()
    {
        BodyRenderer.material = MonsterMaterialDamage;
        HatRenderer.material = MonsterMaterialDamage;
        anim.SetTrigger("Damage");
        StartCoroutine(WaitDamageAnimation());
    }

    private IEnumerator WaitDamageAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            BodyRenderer.material = MonsterMaterialNormal[matIndex - 1];
            HatRenderer.material = MonsterMaterialNormal[matIndex - 1];
        }
    }

    public void Dead()
    {
        anim.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }

    public void DestroyMonster()
    {
        Destroy(gameObject, 0.0f);
    }
}
