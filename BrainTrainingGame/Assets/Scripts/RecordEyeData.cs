using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class RecordEyeData : MonoBehaviour
{
    public void EyeCalibration()
    {
        SRanipal_Eye.LaunchEyeCalibration();
    }



}
