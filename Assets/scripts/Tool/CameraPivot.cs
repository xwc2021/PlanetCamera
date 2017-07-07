
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

 

public class CameraPivot : MonoBehaviour, SurfaceFollowCameraBehavior
{
    public bool doCameraCollison = true;
    float fixedR;
    static float cameraCollisionMinDistance = 1.25f;
    public Transform fakeCamera;
    public Transform player;
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
    Quaternion cameraTargetRot;
    Transform myParent;
    Transform CAMERA;
    Camera c;

    public bool lockYaw = false;

    float R;

    public float RScale = 1;
    float targetRScale=1;
    public void resetTargetRScale(float s)
    {
        targetRScale = s;
    }

    public void resetRScale()
    {
        RScale = 1;
        targetRScale = 1;
    }

    static public float rotateMaxBorader=240;
    static public float rotateMinBorader=-60;
    public float localNowPitchDegree;
    public bool flyAway = false;

    // Use this for initialization
    void Start () {
        myParent = transform.parent;
        cameraTargetRot = myParent.rotation;
        CAMERA = transform.GetChild(0);
        c=CAMERA.GetComponent<Camera>();
        recordPos = transform.position;
        R = Mathf.Abs(CAMERA.localPosition.z);

        if (doCameraCollison)
            fixedR = Mathf.Abs(fakeCamera.localPosition.z);
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
            Vector3 old = recordPos;
            recordPos = Vector3.Lerp(recordPos, myParent.position, posFollowSpeed * Time.deltaTime);
            
            float diff = (old - recordPos).magnitude;
            //print(diff);
            if(autoYawFollow)
                doYawFollow = (diff > doYawFollowDiff) ? true : false;//超過門檻值才作

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
            cameraTargetRot = yaw * cameraTargetRot;

            //doYawFollow
            if (autoYawFollow && doYawFollow)
            {
                Quaternion y = Quaternion.Slerp(Quaternion.identity, autoYawFollowTurnDiff, yawFollowSpeed * Time.deltaTime);
                cameraTargetRot = y * cameraTargetRot;
            }
        }      

        Quaternion surfaceFollow = Quaternion.identity;
        //因為可以在曲面(球、甜甜圈、knock)上移動，所以要有這一項的修正
        if (doSurfaceFollow)
        {
            temporarySurfaceRot = Quaternion.Slerp(temporarySurfaceRot, sumSurfaceRot, rotateFollowSpeed * Time.deltaTime);
            surfaceFollow = temporarySurfaceRot;
        }

        transform.rotation = surfaceFollow * cameraTargetRot * pitch;

        //calculate yawFollow
        if (autoYawFollow)
        {
            if (doYawFollow)
            {
                //使用cameraForwardOnPlane和targetForward的夾角作為yawFollow的旋轉量
                Quaternion final = sumSurfaceRot * cameraTargetRot * pitch;//使用sumSurfaceRot來計算!
                Vector3 cameraForwardInWorld = final * Vector3.forward;
                Vector3 planeNormal = myParent.up;
                Vector3 cameraForwardOnPlane = Vector3.ProjectOnPlane(cameraForwardInWorld, planeNormal);
                cameraForwardOnPlane.Normalize();

                //camera繞過頭頂的情況
                if (localNowPitchDegree > 90)
                    cameraForwardOnPlane = -cameraForwardOnPlane;

                Vector3 targetForward = myParent.forward;

                Vector3 helpV = Vector3.Cross(cameraForwardOnPlane, targetForward);
                float dotValue = Vector3.Dot(cameraForwardOnPlane, targetForward);
                float sign = Vector3.Dot(helpV, planeNormal) > 0.0f ? 1.0f : -1.0f;
                yawFollowDegree = Mathf.Acos(dotValue) * Mathf.Rad2Deg;

                //在限定的範圍內才作yawFollow
                bool doAdjust = yawFollowDegree > yawFollowMin && yawFollowDegree < yawFollowMax;
                autoYawFollowTurnDiff = doAdjust ? Quaternion.AngleAxis(sign * yawFollowDegree, recordParentInitUp) : Quaternion.identity;

                //draw debug
                Vector3 pPos = myParent.transform.position;
                float scale = 5.0f;
                Debug.DrawLine(pPos, pPos + scale * targetForward, Color.yellow);
                Debug.DrawLine(pPos, pPos + scale * cameraForwardOnPlane, Color.blue);
            }
            else
                autoYawFollowTurnDiff = Quaternion.identity;
        }

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
            //Endless Corrider Scene縮放player時所以也要跟著縮放R
            RScale = Mathf.Lerp(RScale, targetRScale, posFollowSpeed * Time.deltaTime);
            adjustR();

            if(doCameraCollison)
                doCameraCollision();
        }
    }

    void doCameraCollision()
    {
        //camera碰撞
        Vector3 cameraPos = fakeCamera.position;
        float halfFovRad = 0.5f * c.fieldOfView * Mathf.Deg2Rad;
        float halfH = Mathf.Tan(halfFovRad);
        Vector3 cameraCenterBottom = cameraPos + (fakeCamera.forward - fakeCamera.up * halfH) * c.nearClipPlane;
        //Debug.DrawLine(CAMERA.transform.position, cameraCenterBottom,Color.green);

        float ep = 0.1f;
        Vector3 from = player.position+player.up* ep;//從3D model的底部開始
        Vector3 dir = cameraCenterBottom - from;
        float rayCastDistance = dir.magnitude;
        dir.Normalize();

        // ground | block
        int layerMask = 1 << 10 | 1 << 14;
        RaycastHit hit;
        float distance = 0;
        Debug.DrawLine(from, cameraCenterBottom, Color.green);
        if (Physics.Raycast(from, dir, out hit, rayCastDistance, layerMask))
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow);
            Vector3 diff = from - hit.point;
            bool underPlane = Vector3.Dot(diff, hit.normal)<0.0f;
            //當player跳起後落地時，有可能穿過地板
            if (underPlane)
            {
                //待辦：這裡再發射第2次看有沒有碰到其他東西
                float offset = 0.1f;
                from = hit.point+dir*offset;
                if (Physics.Raycast(from, dir, out hit, rayCastDistance, layerMask))
                {
                    print("第2次");
                }
                else
                {
                    print("exclude:underPlane");
                    return;
                }     
            }

            distance = Vector3.Dot(hit.point - cameraPos, CAMERA.forward);
            float finalR=Mathf.Min(fixedR - distance, R);
            finalR = Mathf.Max(finalR, cameraCollisionMinDistance);
            CAMERA.localPosition = new Vector3(0, 0, -finalR * RScale);
             //print("hit"+ finalR);
        }
    }

    void adjustR()
    {
        float Rscale = Input.GetAxis("Mouse ScrollWheel");
        R += Rdiff * Rscale * Time.deltaTime;
        R = Mathf.Max(limitR, R);

        CAMERA.localPosition = new Vector3(0, 0, -R * RScale);
        Debug.DrawLine(transform.position, CAMERA.position, Color.red);
    }

    Quaternion temporarySurfaceRot = Quaternion.identity;
    Quaternion sumSurfaceRot = Quaternion.identity;
    bool doSurfaceFollow = false;

    void SurfaceFollowCameraBehavior.setAdjustRotate(bool doRotateFollow, Quaternion adjustRotate)
    {
        sumSurfaceRot = adjustRotate* sumSurfaceRot;
        this.doSurfaceFollow = doRotateFollow;
    }

    public bool autoYawFollow = false;
    bool doYawFollow=false;
    Quaternion autoYawFollowTurnDiff= Quaternion.identity;
    public  float doYawFollowDiff = 0.2f;
    public float yawFollowDegree;
    public float yawFollowMin = 30.0f;
    public float yawFollowMax = 95.0f;
}
