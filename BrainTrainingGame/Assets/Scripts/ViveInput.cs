using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveInput : MonoBehaviour
{
    public SteamVR_Action_Single SqeezeAction;
    public SteamVR_Action_Vector2 TouchAction;

    private void Awake()
    {
        //m_BooleanAction = SteamVR_Actions._default.GrabPinch;
    }

    private void Update()
    {
        float triggerValue = SqeezeAction.GetAxis(SteamVR_Input_Sources.Any);
        Vector2 touchpadValue = TouchAction.GetAxis(SteamVR_Input_Sources.Any);

        if(SteamVR_Actions._default.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            //print("teleport down");
        }

        if(SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
        {
            print("grab pinch down: ");
            print(triggerValue);
        }

        //if(SteamVR_Actions._default.SnapTurnLeft.GetStateDown(SteamVR_Input_Sources.Any))
        //{
        //    print("Turn Left: ");
        //    print(touchpadValue);
        //}
        //
        //if(SteamVR_Actions._default.SnapTurnRight.GetStateDown(SteamVR_Input_Sources.Any))
        //{
        //    print("Turn Right: ");
        //    print(touchpadValue);
        //}

        


    }
}
