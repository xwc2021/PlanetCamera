
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CameraPivot : MonoBehaviour
{
    /* 第1人稱Camera相關 */
    public Transform firstPersonModel;
    public bool firstPersonMode = false;
    void syncFirstPersonModelRotation()
    {
        //更新模型轉向
        Vector3 right = Vector3.Cross(firstPersonModel.up, transform.forward);
        Vector3 forward = Vector3.Cross(right, firstPersonModel.up);
        Quaternion q = Quaternion.LookRotation(forward, firstPersonModel.up);
        firstPersonModel.rotation = q;
    }

    /* camera跟隨相關 */
    Vector3 recordParentInitUp;
    Vector3 recordPos;
    Quaternion cameraTargetRot;
    public Transform cameraTarget; // camera跟隨的目標
    public bool posFollowLerp = true;
    float posFollowSpeed = 5;

    public float yawFollowSpeed = 1.5f;
    public float rotateFollowSpeed = 5;

    /* camera碰撞相關 */
    public bool isDoCameraCollision = true;
    public Transform collisionRayTarget;
    public Transform fakeCamera; // 處理cameraCollison，實際發射線的位置
    Transform realCamera;
    Camera c;

    /* camera距離相關 */
    float fixedR;// camera沒有發生Collison時的距離
    public float cameraMinDistance = 1.0f;
    float R; // Camera 到 cameraTarget的距離
    public float Rdiff = 300;

    // 角色變小時，會用RScale對R做修正
    public float RScale = 1;
    public void resetTargetRScale(float s) { RScale = s; }
    public void resetRScale() { RScale = 1; }

    /* yaw、pitch相關 */
    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;
    public bool lockYaw = false;

    static public float rotateMaxBorader = 240;
    static public float rotateMinBorader = -60;
    public float localNowPitchDegree;

    // Use this for initialization
    void Awake()
    {
        Debug.Assert(collisionRayTarget != null);

        cameraTarget = transform.parent;
        cameraTargetRot = cameraTarget.rotation;
        realCamera = transform.GetChild(0);
        c = realCamera.GetComponent<Camera>();
        recordPos = transform.position;
        R = Mathf.Abs(realCamera.localPosition.z);

        if (isDoCameraCollision)
            fixedR = Mathf.Abs(fakeCamera.localPosition.z);
        recordParentInitUp = cameraTarget.up;

        //記錄一開始的pitch值
        localNowPitchDegree = transform.localRotation.eulerAngles.x;
        setFollowSpeed(false);
    }


    public void resetRecordPos(Vector3 player, float scaleR)
    {
        var offset = (recordPos - player);
        var k = 0.0f; // 如果要從遠處zoom in 可以調這個
        recordPos = (scaleR + k) * offset;

        // 之前畫面會閃1下，就是因為沒有重設postion，距離變化太大造成的
        // 但直接重設postion，還是會有頓1下的感覺
        transform.position = recordPos;
    }

    float modifyPitch(float deltaPitch)
    {
        float newPitchDegree = localNowPitchDegree + deltaPitch;

        //加上Pitch的邊界檢查
        newPitchDegree = Mathf.Min(newPitchDegree, rotateMaxBorader);
        newPitchDegree = Mathf.Max(newPitchDegree, rotateMinBorader);

        localNowPitchDegree = newPitchDegree;
        return localNowPitchDegree;
    }

    public void adjustYaw(float degree)
    {
        Quaternion yaw = Quaternion.AngleAxis(degree, recordParentInitUp);
        cameraTargetRot = yaw * cameraTargetRot;
    }

    private void FixedUpdate()
    {
        updateCamera();
    }

    private void Start()
    {
        transform.parent = null;
    }

    // 平台有開啟cameraFollowUsingHighSpeed時
    // 跳上平台，讓Camera加速，會產生一種著地速度感
    public void setFollowSpeed(bool useHighSpeed)
    {
        if (!posFollowLerp)
            return;

        if (useHighSpeed)
            hopeSpeed = 300;
        else
            hopeSpeed = 5;
    }

    void doPosFollow(out bool doYawFollow)
    {
        Vector3 old = recordPos;

        if (posFollowLerp)
        {
            posFollowSpeed = Mathf.Lerp(posFollowSpeed, hopeSpeed, Time.deltaTime);
            recordPos = Vector3.Lerp(recordPos, cameraTarget.position, posFollowSpeed * Time.deltaTime);
        }
        else
            recordPos = cameraTarget.position;

        float diff = (old - recordPos).magnitude;
        //print(diff);
        if (autoYawFollow)
            doYawFollow = diff > doYawFollowDiff;//set a threshold
        else
            doYawFollow = false;

        if (diff > 0.0001f)
            transform.position = recordPos;
    }

    void calculateAutoYawFollowTurnDiff(bool doYawFollow, Quaternion final)
    {
        if (doYawFollow)
        {
            //使用cameraForwardOnPlane和targetForward的夾角作為yawFollow的旋轉量
            Vector3 cameraForwardInWorld = final * Vector3.forward;
            Vector3 planeNormal = cameraTarget.up;
            Vector3 cameraForwardOnPlane = Vector3.ProjectOnPlane(cameraForwardInWorld, planeNormal);
            cameraForwardOnPlane.Normalize();

            //camera繞過頭頂的情況
            if (localNowPitchDegree > 90)
                cameraForwardOnPlane = -cameraForwardOnPlane;

            Vector3 targetForward = cameraTarget.forward;

            Vector3 helpV = Vector3.Cross(cameraForwardOnPlane, targetForward);
            float dotValue = Vector3.Dot(cameraForwardOnPlane, targetForward);
            float sign = Vector3.Dot(helpV, planeNormal) > 0.0f ? 1.0f : -1.0f;
            yawFollowDegree = Mathf.Acos(dotValue) * Mathf.Rad2Deg;

            //在限定的範圍內才作yawFollow
            bool doAdjust = yawFollowDegree > yawFollowMin && yawFollowDegree < yawFollowMax;
            autoYawFollowTurnDiff = doAdjust ? Quaternion.AngleAxis(sign * yawFollowDegree, recordParentInitUp) : Quaternion.identity;

            //draw debug
            Vector3 pPos = cameraTarget.transform.position;
            float scale = 5.0f;
            Debug.DrawLine(pPos, pPos + scale * targetForward, Color.yellow);
            Debug.DrawLine(pPos, pPos + scale * cameraForwardOnPlane, Color.blue);
        }
        else
            autoYawFollowTurnDiff = Quaternion.identity;
    }

    public float hopeSpeed;
    void updateCamera()
    {
        var inputProxy = InputManager.getInputProxy();
        float deltaY = -CrossPlatformInputManager.GetAxis("Mouse Y") * inputProxy.pitchScale();
        float deltaX = CrossPlatformInputManager.GetAxis("Mouse X") * inputProxy.yawScale();

        Debug.Assert(inputProxy != null);

        bool doYawFollow;
        doPosFollow(out doYawFollow);

        if (!lockYaw)
        {
            //yaw旋轉
            Quaternion yaw = Quaternion.AngleAxis(perYawDegreen * deltaX * Time.deltaTime, recordParentInitUp);
            cameraTargetRot = yaw * cameraTargetRot;

            //doYawFollow by autoYawFollowTurnDiff
            if (autoYawFollow && doYawFollow)
            {
                Quaternion y = Quaternion.Slerp(Quaternion.identity, autoYawFollowTurnDiff, yawFollowSpeed * Time.deltaTime);
                cameraTargetRot = y * cameraTargetRot;
            }
        }

        // pitch旋轉
        Quaternion pitch = Quaternion.Euler(modifyPitch(perPitchDegreen * deltaY * Time.deltaTime), 0, 0);
        transform.rotation = getTemporarySurfaceFollowModify() * cameraTargetRot * pitch;

        if (autoYawFollow)
        {
            Quaternion final = sumSurfaceRot * cameraTargetRot * pitch;//使用sumSurfaceRot來計算!
            calculateAutoYawFollowTurnDiff(doYawFollow, final);
        }

        if (!firstPersonMode)
        {
            adjustDistanceToTarget();

            if (isDoCameraCollision)
                doCameraCollision();
        }
        else
            syncFirstPersonModelRotation();
    }

    Quaternion getTemporarySurfaceFollowModify()
    {
        Quaternion surfaceFollow = Quaternion.identity;
        //因為可以在曲面(球、甜甜圈、knock)上移動，所以要有這一項的修正
        if (doSurfaceFollow)
        {
            temporarySurfaceRot = Quaternion.Slerp(temporarySurfaceRot, sumSurfaceRot, rotateFollowSpeed * Time.deltaTime);
            surfaceFollow = temporarySurfaceRot;
        }
        return surfaceFollow;
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
        Vector3 from = collisionRayTarget.position + collisionRayTarget.up * ep;//從3D model的底部開始
        Vector3 dir = cameraCenterBottom - from;
        float rayCastDistance = dir.magnitude;
        dir.Normalize();

        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.wall;
        RaycastHit hit;
        float distance = 0;
        //Debug.DrawLine(from, cameraCenterBottom, Color.green);
        if (Physics.Raycast(from, dir, out hit, rayCastDistance, layerMask))
        {
            //Debug.DrawRay(hit.point,  hit.normal, Color.red);
            //Debug.DrawRay(hit.point + hit.normal, 0.25f*hit.normal, Color.green);
            Vector3 diff = from - hit.point;
            bool underPlane = Vector3.Dot(diff, hit.normal) < 0.0f;
            //當player跳起後落地時，有可能穿過地板
            if (underPlane)
            {
                //待辦：這裡再發射第2次看有沒有碰到其他東西
                float offset = 0.1f;
                from = hit.point + dir * offset;
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

            distance = Vector3.Dot(hit.point - cameraPos, realCamera.forward);
            float finalR = Mathf.Min(fixedR - distance, R);
            finalR = Mathf.Max(finalR, cameraMinDistance);
            realCamera.localPosition = new Vector3(0, 0, -finalR * RScale);
            //print("hit"+ finalR);
        }
    }

    void adjustDistanceToTarget()
    {
        float Rscale = Input.GetAxis("Mouse ScrollWheel");
        R += Rdiff * Rscale * Time.deltaTime;
        R = Mathf.Max(cameraMinDistance, R);

        Vector3 newPos = new Vector3(0, 0, -R * RScale);

        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, newPos, posFollowSpeed * Time.deltaTime);
        Debug.DrawLine(transform.position, realCamera.position, Color.red);
    }

    Quaternion temporarySurfaceRot = Quaternion.identity;
    Quaternion sumSurfaceRot = Quaternion.identity;
    bool doSurfaceFollow = false;

    public void setSurfaceRotate(bool doRotateFollow, Quaternion adjustRotate)
    {
        sumSurfaceRot = adjustRotate * sumSurfaceRot;
        this.doSurfaceFollow = doRotateFollow;
    }

    public bool autoYawFollow = false;
    Quaternion autoYawFollowTurnDiff = Quaternion.identity;
    public float doYawFollowDiff = 0.2f;
    public float yawFollowDegree;
    public float yawFollowMin = 30.0f;
    public float yawFollowMax = 95.0f;
}
