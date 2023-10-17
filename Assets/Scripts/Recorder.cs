using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Recorder : MonoBehaviour {


    public Transform SceneTarget;
    public Transform SceneCamera;
    public Transform SceneController;

    public Transform SceneCursorPivotPoint;
    public Transform[] Targets;
    //public string ParticipantNumber;

    private string FilePathSummary;
    private string FilePathAll;
    private string FilePathFitt;

    public float targetSize;
    public float targetDistance;
    public int trialNumber;
    public int repetationNumber;
    public int jitterTargetRange;
    //public int jitterCursorRange;

    public bool isInside = false;

    private int clickCounter = 0;
    private float previousTime;

    private float fittsPreviousTime=0;
    private Vector3 lastHitPoint;
    private Vector3 lastControllerPos;
    private float currentTargetPosition;
    private Vector3 lastTargetCenter;
    private string interactionMethod;
    public string currentVACCondition;
    public string gripStyle;


    public Transform SceneCursor;

    public Transform fittsLawHandler;

    public int totalError;

    [Header("CHANGE IT! DONT FORGET")]
    public string ParticipantNumber;

    private string targetRelativePos;

    // Use this for initialization
    void Start () {

        /*
        string path = Application.dataPath + "/ParticipantInfo/ParticipantNo.txt";

        using (StreamReader sr = new StreamReader(path))
        {
            //This allows you to do one Read operation.
            ParticipantNumber = sr.ReadToEnd();
            Debug.Log(ParticipantNumber);
        }
        */

        // fittsLawHandler.GetComponent<FittsLawHandlerForDepthTargets>().setParticipantNumber(ParticipantNumber);

        string FileName = ParticipantNumber + "_" + DateTime.Now.ToString("h-mm-ss") + "_" + "_VirtualHand.txt";

        FilePathAll = Application.persistentDataPath +  "/Resources/All_" + FileName;
        FilePathSummary = Application.persistentDataPath + "/Resources/Summary" + FileName;
        FilePathFitt = Application.persistentDataPath + "/Resources/Fitts_" + FileName;


        string message = "ParticipantNumber;  targetSize; targetDistance; InteractionMethod; targetRelativePos;" +
            "VACCondition; GripStyle;" +
            "repetationNumber; trialNumber;  cursorPos; stableCursorPos; TargetPos; StableTargetPosition; isInside;" +
            "cameraPos; controllerPos; cameraRot; clickCounter; " +
            "Time";

        using (StreamWriter sw = File.AppendText(FilePathAll))
        {
            sw.WriteLine(message);
        }

        message = "ParticipantNumber;  targetSize; targetDistance; method; targetRelativePos;" +
            "TargetPositions;" +
            "repetitionNumber; interactionMethod; GripStyle;" +
            "trialNumber; CursorPosition; indexofDifficulty; targetPosition;" +
            "cursorRot; isInside; cameraPosition;  controllerPosition; controllerRot;" +
            "totalClick; TotalTime ;time_second; time_ms";

        using (StreamWriter sw = File.AppendText(FilePathSummary))
        {
            sw.WriteLine(message);
        }

        message = "ParticipantNumber; targetSize; targetDistance;indexOfDifficulty;" +
            "interactionMethod; VACCondition; GripStyle; targetRelativePos;" +
            "repetationNumber; trialNumber;effectiveTargetWidth; effctiveDistance;" +
            "effctiveDistanceAlternative; (a_dx); dx; Abs_dx;" +
            "effectiveTargetWidth2D; effctiveDistance2D; " +
            "effctiveDistanceAlternative2D; (a_dx2D); dx2D; Abs_dx2D;" +
            "_effectiveTargetWidth; _effctiveDistance; _effctiveDistanceAlternative; (_a_dx); _dx; _Abs_dx;" +
            "_effectiveTargetWidth2D;__effctiveDistance2D; effectiveDistanceAlternative2D; _(a_dx2D); _dx2D; _Abs_dx2D;" +
            "AngulareffectiveDistance; AngularEffectiveTargetWidth;" +
            "clickCounter; isInside; Time (s); Time (ms) "; 


        using (StreamWriter sw = File.AppendText(FilePathFitt))
        {
            sw.WriteLine(message);
        }

    }

    public void setParticipantNumber(string partNumber)
    {
        ParticipantNumber = partNumber;
        Debug.Log("PARTICIPANT NUMBER : " + ParticipantNumber);
    }

    public void setTargetSize(float tarSize)
    {
        targetSize = tarSize;
    }

    public void setTargetDistance(float tarDistance)
    {
        targetDistance = tarDistance;
    }

    public void setRepetationNumber(int repNumber)
    {
        repetationNumber = repNumber;
    }

    public void setTrialNumber(int repNumber)
    {
        trialNumber = repNumber;
    }

    public void setGripStyle(String gripName)
    {
        gripStyle = gripName;
    }

    public void targetNameSet (Vector3 theNameOfTarget){
        targetRelativePos = theNameOfTarget.z.ToString();
        
    }



    public void setInteractionMethodRecorder(String inputTuru)
    {
        interactionMethod = inputTuru;
    }

    public void setVACCondition (String setVal)
    {
        currentVACCondition = setVal;
    }

    public void setTotalErrorZero()
    {

        totalError = 0;
    }

    public void summaryWriter()
    {
        clickCounter++;
        onlyFitts();
        float indexofDifficulty = Mathf.Log(targetDistance / (targetSize) + 1, 2);

        int iceride = 0;
        if (isInside)
        {
            iceride = 1;
  
        } else {
            iceride = 0;
            totalError++;
        }

        string positions = "";

        foreach (Transform trans in Targets)
        {
            positions = positions + trans.position.ToString("G5");
        }

        string message = ParticipantNumber + ";" +
            SceneManager.GetActiveScene().name + ";" +
            targetSize + ";" +
            targetDistance + ";" +
            positions + ";" +
            repetationNumber + ";" +
            interactionMethod + ";" +
            targetRelativePos + ";" +
            currentVACCondition + ";" +
            gripStyle + ";" +
            trialNumber + ";" + 
            SceneCursor.position.ToString("G7") + ";" +
            indexofDifficulty + ";" +
            SceneTarget.position.ToString("G7") + ";" +
            SceneCursorPivotPoint.eulerAngles.ToString("G7") + ";" + 
            iceride + ";" +
            SceneCamera.position.ToString("G5") + ";" + 
            SceneController.position.ToString("G5") + ";" +
            SceneController.rotation.eulerAngles.ToString("G7") + ";" +
            clickCounter + ";" +
            Time.time + ";" +
            (Time.time - previousTime) + ";" +
            (Time.time - previousTime)*1000;

        previousTime = Time.time;

        //Debug.Log(message);

        using (StreamWriter sw = File.AppendText(FilePathSummary))
        {
            sw.WriteLine(message);
        }
    }



    public void onlyFitts()
    {
        if (clickCounter == 1)
        {
            return;
        }


        float _effectiveTargetWidth = Vector3.Distance(SceneCursor.position, SceneTarget.position);
        float _effectiveTargetWidth2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(SceneTarget.position.x, SceneTarget.position.y));
        float _effctiveDistance = Vector3.Distance(SceneCursor.position, lastHitPoint);
        float _effctiveDistance2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(lastHitPoint.x, lastHitPoint.y));
        float _effctiveDistanceAlternative = Vector3.Distance(SceneCursor.position, lastTargetCenter);
        float _effctiveDistanceAlternative2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(lastTargetCenter.x, lastTargetCenter.y));
        float _a = Vector3.Distance(SceneTarget.position, lastTargetCenter);
        float _a2D = Vector2.Distance(new Vector2(SceneTarget.position.x, SceneTarget.position.y), new Vector2(lastTargetCenter.x, lastTargetCenter.y));
        float _c = Vector3.Distance(SceneCursor.position, lastTargetCenter);
        float _c2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(lastTargetCenter.x, lastTargetCenter.y));
        float _b = Vector3.Distance(SceneCursor.position, SceneTarget.position);
        float _b2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(SceneTarget.position.x, SceneTarget.position.y));
        float _dx = (_c * _c - _b * _b - _a * _a) / (2 * _a);
        float _dx2D = (_c2D * _c2D - _b2D * _b2D - _a2D * _a2D) / (2 * _a2D);

        float effectiveTargetWidth = Vector3.Distance(SceneCursor.position, SceneTarget.position);
        float effctiveDistance = Vector3.Distance(SceneCursor.position, lastHitPoint);
        float a = Vector3.Distance(SceneTarget.position, lastHitPoint);
        float b = Vector3.Distance(SceneCursor.position, SceneTarget.position); 
        float c = Vector3.Distance(SceneCursor.position, lastHitPoint);
        float dx = (c * c - b * b - a * a) / (2 * a);
        float effectiveTargetWidth2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(SceneTarget.position.x, SceneTarget.position.y));
        float effctiveDistance2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(lastHitPoint.x, lastHitPoint.y));
        float a2D = Vector2.Distance(new Vector2(SceneTarget.position.x, SceneTarget.position.y), new Vector2(lastHitPoint.x, lastHitPoint.y));
        float b2D =Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(SceneTarget.position.x, SceneTarget.position.y)); 
        float c2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(lastHitPoint.x, lastHitPoint.y));
        float dx2D = (c2D * c2D - b2D * b2D - a2D * a2D) / (2 * a2D);
        float effctiveDistanceAlternative = Vector3.Distance(SceneCursor.position, lastTargetCenter);
        float effctiveDistanceAlternative2D = Vector2.Distance(new Vector2(SceneCursor.position.x, SceneCursor.position.y), new Vector2(lastTargetCenter.x, lastTargetCenter.y));

        Vector3 controllerToPrevious = lastHitPoint - lastControllerPos;
        Vector3 controllerToTarget = SceneCursor.position - SceneController.position;
        float angularEffectiveDistance = Vector3.Angle(controllerToPrevious, controllerToTarget);
        float angularEffectiveWidth = Vector3.Angle(SceneTarget.position - SceneController.position, SceneCursor.position - SceneController.position);

        int iceride = 0;
        if (isInside)
        {
            iceride = 1;
        }
        else
        {
            iceride = 0;
        }
        float indexofDifficulty = Mathf.Log(targetDistance  / (targetSize) + 1, 2);
        string message = ParticipantNumber + ";" +
            targetSize + ";" +
            targetDistance  + ";" +
            indexofDifficulty + ";" +
            interactionMethod + ";" +
            currentVACCondition + ";" +
            gripStyle + ";" +
            targetRelativePos + ";" +
            repetationNumber + ";" +
            trialNumber + ";" +
            
            effectiveTargetWidth + ";" +
            effctiveDistance + ";" +
            effctiveDistanceAlternative + ";" +
            (a + dx) + ";" +
            dx + ";" +
            Math.Abs(dx) + ";" +
                 
            effectiveTargetWidth2D + ";" +
            effctiveDistance2D + ";" +
            effctiveDistanceAlternative2D + ";" +
            (a2D + dx2D) + ";" +
            dx2D + ";" +
            Math.Abs(dx2D) + ";" +

            _effectiveTargetWidth + ";" +
            _effctiveDistance + ";" +
            _effctiveDistanceAlternative + ";" +
            (_a + _dx) + ";" +
            _dx + ";" +
            Math.Abs(_dx) + ";" +
            
            _effectiveTargetWidth2D + ";" +
            _effctiveDistance2D + ";" +
            _effctiveDistanceAlternative2D + ";" +
            (_a2D + _dx2D) + ";" +
            _dx2D + ";" +
            Math.Abs(_dx2D) + ";" +

            angularEffectiveDistance + ";" +
            angularEffectiveWidth + ";" +

            clickCounter + ";" +
            iceride + ";" +
            (Time.time - fittsPreviousTime) + ";" +
            (Time.time - fittsPreviousTime)*1000;

        fittsPreviousTime = Time.time;

        using (StreamWriter sw = File.AppendText(FilePathFitt))
        {
            sw.WriteLine(message);
        }

        lastControllerPos = SceneController.position;
        lastHitPoint = SceneCursor.position;
        lastTargetCenter = SceneTarget.position;
    }


    // Update is called once per frame
    void Update () {
        
		string message = ParticipantNumber + ";" +
            SceneManager.GetActiveScene().name + ";" +
            targetSize + ";" +
            targetDistance + ";" +
            interactionMethod + ";" +
            targetRelativePos + ";" +
            currentVACCondition + ";" +
            gripStyle + ";" +
            repetationNumber + ";" +
            trialNumber + ";" +
            SceneCursor.position.ToString("G5") + ";" +
            SceneTarget.position.ToString("G5") + ";" +
            SceneController.eulerAngles.ToString("G5") + ";" +
            isInside + ";" +
            SceneCamera.position.ToString("G5") + ";" + 
            SceneController.position.ToString("G5") + ";" + 
            SceneCamera.eulerAngles.ToString("G5") + ";" +
            clickCounter + ";" +
            Time.time;

        using (StreamWriter sw = File.AppendText(FilePathAll))
        {
            sw.WriteLine(message);
        }
    }
}
