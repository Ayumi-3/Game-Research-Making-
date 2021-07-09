using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;

public class EyeDataRecord : MonoBehaviour
{
    public Text FocusText;
    private static DataManager dataManager;
    private static CommunicationController communicationController;
    private static Camera mainCamera;
    private static GetPlayerName getPlayerName;

    private static EyeData_v2 eyeData = new EyeData_v2();
    private static bool eye_callback_registered = false;
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };

    public static string FileName;
    public static string DataDir;
    private static bool isRecord = false;
    private static bool isFirst = true;
    //private static string

    private static Dictionary<string, string> eyeTrackingData = new Dictionary<string, string>();

    private void Start()
    {
        dataManager = GetComponent<DataManager>();
        communicationController = GetComponent<CommunicationController>();
        mainCamera = GameObject.FindGameObjectWithTag("VrCameraRig").GetComponentInChildren<Camera>();

        resetEyeTrackingData();

        // Get player name, get folder path, create folder if not exist
        /*playerName = getPlayerName.GetPlayer();
        dataDir = dataPath + playerName + "/";
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }*/
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }

        if (isRecord)
        {
            foreach (GazeIndex index in GazePriority)
            {
                Ray ray;
                FocusInfo focusInfo = new FocusInfo();
                bool focusStatus = SRanipal_Eye_v2.Focus(index, out ray, out focusInfo, eyeData);
                //Debug.Log("FocusStatus: " + focusStatus);
                eyeTrackingData["FocusStatus"] = focusStatus.ToString();
                if (focusStatus)
                {
                    Debug.Log(focusInfo.collider);
                    eyeTrackingData["GazeIndex"] = index.ToString();
                    eyeTrackingData["RayOriginal"] = ray.origin.ToString();
                    eyeTrackingData["RayDirection"] = ray.direction.ToString();
                    eyeTrackingData["Collider"] = focusInfo.collider.ToString();
                    eyeTrackingData["Distance"] = focusInfo.distance.ToString();
                    eyeTrackingData["Normal"] = focusInfo.normal.ToString();
                    eyeTrackingData["Point"] = focusInfo.point.ToString();
                    eyeTrackingData["Transform.position"] = focusInfo.transform.position.ToString();
                    eyeTrackingData["Transform.rotation"] = focusInfo.transform.rotation.ToString();
                    eyeTrackingData["LeftEyeScreenPoint"] = Camera.main.WorldToScreenPoint(focusInfo.point, Camera.MonoOrStereoscopicEye.Left).ToString();
                    eyeTrackingData["RightEyeScreenPoint"] = Camera.main.WorldToScreenPoint(focusInfo.point, Camera.MonoOrStereoscopicEye.Right).ToString();
                    FocusText.text = Camera.main.transform.InverseTransformPoint(focusInfo.point).ToString();
                    /*if (focusInfo.collider.ToString().Contains("Monster"))
                    {
                        FocusText.text = "Monster";
                    }
                    else if (focusInfo.collider.ToString().Contains("Player"))
                    {
                        FocusText.text = "Player";
                    }
                    else if (focusInfo.collider.ToString().Contains("Target"))
                    {
                        FocusText.text = "Target";
                    }
                    else if (focusInfo.collider.ToString().Contains("Obstacle"))
                    {
                        FocusText.text = "Rock";
                    }*/
                }

                else
                {
                    eyeTrackingData["GazeIndex"] = "0";
                    eyeTrackingData["RayOriginal"] = "0";
                    eyeTrackingData["RayDirection"] = "0";
                    eyeTrackingData["Collider"] = "0";
                    eyeTrackingData["Distance"] = "0";
                    eyeTrackingData["Normal"] = "0";
                    eyeTrackingData["Point"] = "0";
                    eyeTrackingData["Transform.position"] = "0";
                    eyeTrackingData["Transform.rotation"] = "0";
                    eyeTrackingData["LeftEyeScreenPoint"] = "0";
                    eyeTrackingData["RightEyeScreenPoint"] = "0";
                    FocusText.text = "Others";
                }

                eyeTrackingData["Camera.position"] = mainCamera.transform.position.ToString();
                eyeTrackingData["Camera.rotation"] = mainCamera.transform.rotation.ToString();

            }
        }
        
    }

    private void OnDisable()
    {
        Release();
    }

    void OnApplicationQuit()
    {
        Release();
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    /// <summary>
    /// Required class for IL2CPP scripting backend support
    /// </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        VerboseData verboseData;
        string dataDir;
        string fileName;
        dataDir = GameControl.Instance.dataDir;
        fileName = GameControl.Instance.eyeTrackingFile;
        if(isRecord)
        {
            eyeData = eye_data;
            //GameControl.Instance.RecordEyeTrackingData(false, eyeData);
            verboseData = eyeData.verbose_data;
            eyeTrackingData["GtecTime"] = communicationController.ReceivedData.ToString();
            eyeTrackingData["UnityTimeTicks"] = System.DateTime.Now.Ticks.ToString(); //(Time.time * 1000).ToString();  //System.DateTime.Now.ToString("HH-mm-ss.fff");
            eyeTrackingData["EyeTimeStamp"] = eyeData.timestamp.ToString();
            eyeTrackingData["CombinedEyeValidDataBitMask"] = verboseData.combined.eye_data.eye_data_validata_bit_mask.ToString();
            eyeTrackingData["LeftEyeValidDataBitMask"] = verboseData.left.eye_data_validata_bit_mask.ToString();
            eyeTrackingData["RightEyeValidDataBitMask"] = verboseData.right.eye_data_validata_bit_mask.ToString();
            eyeTrackingData["CombinedEyeOpenness"] = verboseData.combined.eye_data.eye_openness.ToString();
            eyeTrackingData["LeftEyeOpenness"] = verboseData.left.eye_openness.ToString();
            eyeTrackingData["RightEyeOpenness"] = verboseData.right.eye_openness.ToString();
            eyeTrackingData["CombinedPupilDiameter"] = verboseData.combined.eye_data.pupil_diameter_mm.ToString();
            eyeTrackingData["LeftPupilDiameter"] = verboseData.left.pupil_diameter_mm.ToString();
            eyeTrackingData["RightPupilDiameter"] = verboseData.right.pupil_diameter_mm.ToString();
            eyeTrackingData["CombinedPupilPosition"] = verboseData.combined.eye_data.pupil_position_in_sensor_area.ToString();
            eyeTrackingData["LeftPupilPosition"] = verboseData.left.pupil_position_in_sensor_area.ToString();
            eyeTrackingData["RightPupilPosition"] = verboseData.right.pupil_position_in_sensor_area.ToString();
            eyeTrackingData["CombinedGazeOrigin"] = verboseData.combined.eye_data.gaze_origin_mm.ToString();
            eyeTrackingData["LeftGazeOrigin"] = verboseData.left.gaze_origin_mm.ToString();
            eyeTrackingData["RightGazeOrigin"] = verboseData.right.gaze_origin_mm.ToString();
            eyeTrackingData["CombinedGazeDirection"] = verboseData.combined.eye_data.gaze_direction_normalized.ToString();
            eyeTrackingData["LeftGazeDirection"] = verboseData.left.gaze_direction_normalized.ToString();
            eyeTrackingData["RightGazeDirection"] = verboseData.right.gaze_direction_normalized.ToString();

            eyeTrackingData["LeftEyeWide"] = eyeData.expression_data.left.eye_wide.ToString();
            eyeTrackingData["RightEyeWide"] = eyeData.expression_data.right.eye_wide.ToString();
            eyeTrackingData["LeftEyeSqeeze"] = eyeData.expression_data.left.eye_squeeze.ToString();
            eyeTrackingData["RightEyeSqeeze"] = eyeData.expression_data.right.eye_squeeze.ToString();
            eyeTrackingData["LeftEyeFrown"] = eyeData.expression_data.left.eye_frown.ToString();
            eyeTrackingData["RightEyeFrown"] = eyeData.expression_data.right.eye_frown.ToString();

            if (isFirst)
            {
                dataManager.WriteData(dataDir, fileName, eyeTrackingData, true, true);
                isFirst = false;
            }
            else
            {
                dataManager.WriteData(dataDir, fileName, eyeTrackingData, false, false);
            }
        }

    }

    public void StartRecord()
    {
        isRecord = true;
        isFirst = true;
        resetEyeTrackingData();
    }

    public void StopRecord()
    {
        isRecord = false;
    }

    private void resetEyeTrackingData()
    {
        eyeTrackingData["GtecTime"] = communicationController.ReceivedData.ToString();
        eyeTrackingData["UnityTimeTicks"] = System.DateTime.Now.Ticks.ToString(); //(Time.time * 1000).ToString();  //System.DateTime.Now.ToString("HH-mm-ss.fff");
        eyeTrackingData["EyeTimeStamp"] = "0";
        eyeTrackingData["CombinedEyeValidDataBitMask"] = "0";
        eyeTrackingData["LeftEyeValidDataBitMask"] = "0";
        eyeTrackingData["RightEyeValidDataBitMask"] = "0";
        eyeTrackingData["CombinedEyeOpenness"] = "0";
        eyeTrackingData["LeftEyeOpenness"] = "0";
        eyeTrackingData["RightEyeOpenness"] = "0";
        eyeTrackingData["CombinedPupilDiameter"] = "0";
        eyeTrackingData["LeftPupilDiameter"] = "0";
        eyeTrackingData["RightPupilDiameter"] = "0";
        eyeTrackingData["CombinedPupilPosition"] = "0";
        eyeTrackingData["LeftPupilPosition"] = "0";
        eyeTrackingData["RightPupilPosition"] = "0";
        eyeTrackingData["CombinedGazeOrigin"] = "0";
        eyeTrackingData["LeftGazeOrigin"] = "0";
        eyeTrackingData["RightGazeOrigin"] = "0";
        eyeTrackingData["CombinedGazeDirection"] = "0";
        eyeTrackingData["LeftGazeDirection"] = "0";
        eyeTrackingData["RightGazeDirection"] = "0";

        eyeTrackingData["LeftEyeWide"] = "0";
        eyeTrackingData["RightEyeWide"] = "0";
        eyeTrackingData["LeftEyeSqeeze"] = "0";
        eyeTrackingData["RightEyeSqeeze"] = "0";
        eyeTrackingData["LeftEyeFrown"] = "0";
        eyeTrackingData["RightEyeFrown"] = "0";

        eyeTrackingData["FocusStatus"] = "False";
        eyeTrackingData["GazeIndex"] = "0";
        eyeTrackingData["RayOriginal"] = "0";
        eyeTrackingData["RayDirection"] = "0";
        eyeTrackingData["Collider"] = "0";
        eyeTrackingData["Distance"] = "0";
        eyeTrackingData["Normal"] = "0";
        eyeTrackingData["Point"] = "0";
        eyeTrackingData["Transform.position"] = "0";
        eyeTrackingData["Transform.rotation"] = "0";
        eyeTrackingData["LeftEyeScreenPoint"] = "0";
        eyeTrackingData["RightEyeScreenPoint"] = "0";
        eyeTrackingData["Camera.position"] = "0";
        eyeTrackingData["Camera.rotation"] = "0";
    }
}
