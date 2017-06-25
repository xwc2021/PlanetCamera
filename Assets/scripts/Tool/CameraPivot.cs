
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

 

public class CameraPivot : MonoBehaviour, FollowCameraBehavior
{
    public float yawFollowSpeed = 1.5f;
    public float rotateFollowSpeed = 5;
    public bool follow = true;
    public float posFollowSpeed = 5;
    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;
    public float Rdiff = 300;
    public Transform syncDirTarget;
    public bool firstPersonMode = false;
    public float limitR = 2.0f;
    Vector3 recordParentInitUp;
    Vector3 recordPos;
    Quaternion rot;
    Transform myParent;
    Transform CAMERA;

    public bool lockYaw = false;

    public float RScale=1;
    float R;

    float targetRScale=1;
    public void resetTargetRScale(float s)
    {
        targetRScale = s;
    }

    public void resetRScale(float s)
    {
        RScale = s;
        targetRScale = s;
    }

    Vector3 posDebug;

    static public float rotateMaxBorader=240;
    static public float rotateMinBorader=-60;
    public float localNowPitchDegree;
    public bool flyAway = false;

    // Use this for initialization
    void Start () {
        rot = transform.rotation;
        myParent = transform.parent;
        CAMERA = transform.GetChild(0);
        recordPos = transform.position;
        posDebug = recordPos;
        R = (transform.position - CAMERA.position).magnitude;

        temporaryTargetTurnDiff = Quaternion.identity;

        recordParentInitUp = myParent.up;

        //記錄一開始的pitch值
        localNowPitchDegree = transform.localRotation.eulerAngles.x;

        //只解除parent關系，只要player有縮小就是會搖動
        //還是得在doScale裡鎖scale的增加值
        if (flyAway)
            transform.parent = null;
    }

    public void resetRecordPos(Vector3 v,float scaleR)
    {
        recordPos = scaleR * recordPos+v;
    }

    void addPitch(float deltaPitch)
    {
        float newPitchDegree = localNowPitchDegree + deltaPitch;

        //加上Pitch的邊界檢查
        newPitchDegree = Mathf.Min(newPitchDegree, rotateMaxBorader);
        newPitchDegree = Mathf.Max(newPitchDegree, rotateMinBorader);

        localNowPitchDegree = newPitchDegree;
    }

    void LateUpdate() {

        if (follow)
        {
            recordPos = Vector3.Lerp(recordPos, myParent.position, posFollowSpeed * Time.deltaTime);
            transform.position = recordPos;
        }     

        //pitch旋轉
        float deltaY = -CrossPlatformInputManager.GetAxis("Mouse Y");
        addPitch(perPitchDegreen * deltaY * Time.deltaTime);
        Quaternion pitch = Quaternion.Euler(localNowPitchDegree, 0, 0);

        if (!lockYaw)
        {
            //yaw旋轉
            float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");   
            Quaternion yaw = Quaternion.AngleAxis(perYawDegreen * deltaX * Time.deltaTime, recordParentInitUp);
            rot = yaw * rot;
        }

        Quaternion chain = Quaternion.identity;

        //當avatar轉向時所作的修正
        if (doYawFollow)
        {
            temporaryTargetTurnDiff = Quaternion.Slerp(temporaryTargetTurnDiff, sumTargetTurnDiff, yawFollowSpeed * Time.deltaTime);
            chain = temporaryTargetTurnDiff * chain;
        }

        //因為可以在曲面(球、甜甜圈、knock)上移動，所以要有這一項的修正
        if (doRotateFollow)
        {
            temporaryFinal = Quaternion.Slerp(temporaryFinal, sumAdjustRot, rotateFollowSpeed * Time.deltaTime);
            chain = temporaryFinal * chain;
        }

        transform.rotation = chain*rot*pitch;

        if (firstPersonMode)
        {
            //更新模型轉向
            Vector3 right = Vector3.Cross(syncDirTarget.up, transform.forward);
            Vector3 forward = Vector3.Cross(right, syncDirTarget.up);
            Quaternion q = Quaternion.LookRotation(forward, syncDirTarget.up);
            syncDirTarget.rotation = q;
        }
 
        if (!firstPersonMode)
        { 
            float Rscale = Input.GetAxis("Mouse ScrollWheel");
            R += Rdiff * Rscale * Time.deltaTime;
            R = Mathf.Max(limitR, R);

            RScale = Mathf.Lerp(RScale, targetRScale, posFollowSpeed * Time.deltaTime);
            CAMERA.localPosition = new Vector3(0, 0, -R* RScale);
            Debug.DrawLine(transform.position, CAMERA.position, Color.red);
        }
    }

    Quaternion temporaryFinal = Quaternion.identity;
    Quaternion sumAdjustRot=Quaternion.identity;
    bool doRotateFollow = false;

    void FollowCameraBehavior.setAdjustRotate(bool doRotateFollow, Quaternion adjustRotate)
    {
        sumAdjustRot = adjustRotate*sumAdjustRot;
        this.doRotateFollow = doRotateFollow;
    }

    void FollowCameraBehavior.adjustCameraYaw(float diff)
    {
        //do nothing
    }

    float sumTurnDiff=0;
    bool doYawFollow=false;
    Quaternion temporaryTargetTurnDiff;
    Quaternion sumTargetTurnDiff= Quaternion.identity;
    void FollowCameraBehavior.adjustCameraYaw(bool doYawFollow,float yawDegree)
    {
        yawDegree = yawDegree < 180 ? yawDegree : yawDegree-360;
        sumTurnDiff = (sumTurnDiff + yawDegree)%360.0f;

        sumTargetTurnDiff = Quaternion.AngleAxis(sumTurnDiff, recordParentInitUp);

        this.doYawFollow = doYawFollow;
        print(yawDegree);
    }
}
