
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

    public float rotateMaxBorader=45;
    public float rotateMinBorader=-80;
    public float nowPitchDegree;//-90<PitchDegree<90
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

        //計算一開始的ptich值
        nowPitchDegree = getNowPitchDegree(recordParentInitUp);

        //只解除parent關系，只要player有縮小就是會搖動
        //還是得在doScale裡鎖scale的增加值
        if (flyAway)
            transform.parent = null;
    }

    float getNowPitchDegree(Vector3 PlaneNormal)
    {
        Vector3 x = Vector3.ProjectOnPlane(transform.forward, PlaneNormal);
        Vector3 y = transform.forward - x;
        float sign = Vector3.Dot(PlaneNormal, y) > 0 ? 1 : -1;
        return Mathf.Rad2Deg * Mathf.Atan2(sign * y.magnitude, x.magnitude);
    }

    public void resetRecordPos(Vector3 v,float scaleR)
    {
        recordPos = scaleR * recordPos+v;
    }

    float limitDeltaPitch(float deltaPitch)
    {
        //deltaPitch增加時，會讓nowPitchDegree減少
        float deltaPitchDegree = -deltaPitch;
        float newPitchDegree = nowPitchDegree + deltaPitchDegree;

        //加上Pitch的邊界檢查
        newPitchDegree = Mathf.Min(newPitchDegree, rotateMaxBorader);
        newPitchDegree = Mathf.Max(newPitchDegree, rotateMinBorader);

        deltaPitchDegree = newPitchDegree - nowPitchDegree;
        deltaPitch = -(deltaPitchDegree);

        return deltaPitch;
    }

    void LateUpdate() {

        if (follow)
        {
            recordPos = Vector3.Lerp(recordPos, myParent.position, posFollowSpeed * Time.deltaTime);
            transform.position = recordPos;
        }     

        //rot在local space運作
        float deltaY = -CrossPlatformInputManager.GetAxis("Mouse Y");

        float deltaPitch = limitDeltaPitch(perPitchDegreen * deltaY * Time.deltaTime);
        Quaternion pitch = Quaternion.Euler(deltaPitch, 0, 0);
        rot = rot * pitch;

        if (!lockYaw)
        {
            float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");   
            Quaternion yaw = Quaternion.AngleAxis(perYawDegreen * deltaX * Time.deltaTime, recordParentInitUp);

            rot = yaw * rot;
        }

        Quaternion chain = Quaternion.identity;

        if (doYawFollow)
        {
            temporaryTargetTurnDiff = Quaternion.Slerp(temporaryTargetTurnDiff, sumTargetTurnDiff, yawFollowSpeed * Time.deltaTime);
            chain = temporaryTargetTurnDiff * chain;
        }

        if (doRotateFollow)
        {
            temporaryFinal = Quaternion.Slerp(temporaryFinal, sumAdjustRot, rotateFollowSpeed * Time.deltaTime);
            chain = temporaryFinal * chain;
        }

        transform.rotation = chain*rot;

        if (firstPersonMode)
        {
            //更新模型轉向
            Vector3 right = Vector3.Cross(syncDirTarget.up, transform.forward);
            Vector3 forward = Vector3.Cross(right, syncDirTarget.up);
            Quaternion q = Quaternion.LookRotation(forward, syncDirTarget.up);
            syncDirTarget.rotation = q;
        }

        nowPitchDegree = getNowPitchDegree(myParent.up);

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
