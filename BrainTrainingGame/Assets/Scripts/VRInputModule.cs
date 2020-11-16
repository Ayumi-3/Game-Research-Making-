using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera InputCamera;
    public SteamVR_Input_Sources TargetSource;
    public SteamVR_Action_Boolean ClickAction;

    private GameObject currentObject = null;
    private PointerEventData data = null;

    protected override void Awake()
    {
        base.Awake();

        data = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        // reset data, set camera
        data.Reset();
        data.position = new Vector2(InputCamera.pixelWidth / 2, InputCamera.pixelHeight / 2);

        // raycast
        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        currentObject = data.pointerCurrentRaycast.gameObject;

        // clear
        m_RaycastResultCache.Clear();

        // hover
        HandlePointerExitAndEnter(data, currentObject);

        // press
        if(ClickAction.GetStateDown(TargetSource))
        {
            ProcessPress(data);
        }

        // release
        if(ClickAction.GetStateUp(TargetSource))
        {
            ProcessRelease(data);
        }

    }

    public PointerEventData GetData()
    {
        return data;
    }

    private void ProcessPress(PointerEventData eventData)
    {

    }

    private void ProcessRelease(PointerEventData eventData)
    {

    }

}
