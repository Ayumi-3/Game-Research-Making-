using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeTrackingCalibration : MonoBehaviour
{
    public void EyeTrackingCaribrateOnClick()
    {
        SRanipal_Eye_v2.LaunchEyeCalibration();
    }
}
