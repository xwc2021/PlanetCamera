using UnityEngine;
public class CameraPivot : MonoBehaviour
{
    public Transform getCameraTransform()
    {
        return realCamera;
    }

    public Camera getCamera()
    {
        return c;
    }

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
    float hopePosFollowSpeed;
    float posFollowSpeed;

    /* camera跟隨相關：surface修正 */
    public float surfaceAdjustLerpSpeed = 5;
    Quaternion temporarySurfaceAdjust = Quaternion.identity; // 混合中
    Quaternion sumSurfaceAdjust = Quaternion.identity;
    bool doSurfaceFollow = false;

    public void setSurfaceAdjust(bool doRotateFollow, Quaternion adjustRotate)
    {
        sumSurfaceAdjust = adjustRotate * sumSurfaceAdjust;
        this.doSurfaceFollow = doRotateFollow;
    }

    Quaternion getTemporarySurfaceFollowModify()
    {
        //因為可以在曲面(球、甜甜圈、knock)上移動，所以要有這一項的修正
        if (doSurfaceFollow)
            return temporarySurfaceAdjust = Quaternion.Slerp(temporarySurfaceAdjust, sumSurfaceAdjust, surfaceAdjustLerpSpeed * Time.deltaTime);
        return Quaternion.identity;
    }

    /* camera跟隨相關：移動時自動調整yaw */
    public bool usingAutoYaw = false;
    public float autoYawLerpSpeed = 1.5f;

    public float posDiffThresholdForAutoYaw = 0.2f; // pos diff超過值，才需要autoYaw
    Quaternion autoYawRot = Quaternion.identity;
    public float autoYawAdjustDegree;
    public float autoYawMinDegree = 30.0f;
    public float autoYawMaxDegree = 95.0f;

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

    /* input */
    Vector2 lookVec;
    public Vector2 LookVec // property
    {
        set { lookVec = value; }
    }

    public void bind(Transform cameraTarget, Transform collisionRayTarget)
    {
        this.cameraTarget = cameraTarget;
        cameraTargetRot = cameraTarget.rotation;

        this.transform.position = cameraTarget.transform.position;
        this.transform.rotation = cameraTargetRot;

        this.collisionRayTarget = collisionRayTarget;

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

    // private void Start()
    // {
    //     transform.parent = null;
    // }

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

    // 平台有開啟cameraFollowUsingHighSpeed時
    // 跳上平台，讓Camera加速，會產生一種著地速度感
    public void setFollowSpeed(bool useHighSpeed)
    {
        if (!posFollowLerp)
            return;

        if (useHighSpeed)
            hopePosFollowSpeed = 300;
        else
            hopePosFollowSpeed = 5;
    }

    void doPosFollow(out bool isTriggerYawFollow)
    {
        Vector3 old = recordPos;

        if (posFollowLerp)
        {
            posFollowSpeed = Mathf.Lerp(posFollowSpeed, hopePosFollowSpeed, Time.deltaTime);
            recordPos = Vector3.Lerp(recordPos, cameraTarget.position, posFollowSpeed * Time.deltaTime);
        }
        else
            recordPos = cameraTarget.position;

        float diff = (old - recordPos).magnitude;
        // print(diff);
        if (usingAutoYaw)
            isTriggerYawFollow = diff > posDiffThresholdForAutoYaw;
        else
            isTriggerYawFollow = false;

        if (diff > 0.0001f)
            transform.position = recordPos;
    }

    void calculateAutoYawRot(bool doYawFollow, Quaternion final)
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
            autoYawAdjustDegree = Mathf.Acos(dotValue) * Mathf.Rad2Deg;

            // 限制範圍
            autoYawAdjustDegree = Mathf.Clamp(autoYawAdjustDegree, autoYawMinDegree, autoYawMaxDegree);
            autoYawRot = Quaternion.AngleAxis(sign * autoYawAdjustDegree, recordParentInitUp);

            //draw debug
            Vector3 pPos = cameraTarget.transform.position;
            float scale = 5.0f;
            Debug.DrawLine(pPos, pPos + scale * targetForward, Color.yellow);
            Debug.DrawLine(pPos, pPos + scale * cameraForwardOnPlane, Color.blue);
        }
        else
            autoYawRot = Quaternion.identity;
    }

    void updateCamera()
    {
        float scale = 0.5f;
        float deltaY = -lookVec.y * scale;
        float deltaX = lookVec.x * scale;

        bool isTriggerYawFollow;
        doPosFollow(out isTriggerYawFollow);

        if (!lockYaw)
        {
            //yaw旋轉
            Quaternion yaw = Quaternion.AngleAxis(perYawDegreen * deltaX * Time.deltaTime, recordParentInitUp);
            cameraTargetRot = yaw * cameraTargetRot;

            if (usingAutoYaw && isTriggerYawFollow)
            {
                Quaternion y = Quaternion.Slerp(Quaternion.identity, autoYawRot, autoYawLerpSpeed * Time.deltaTime);
                cameraTargetRot = y * cameraTargetRot;
            }
        }

        // pitch旋轉
        Quaternion pitch = Quaternion.Euler(modifyPitch(perPitchDegreen * deltaY * Time.deltaTime), 0, 0);
        transform.rotation = getTemporarySurfaceFollowModify() * cameraTargetRot * pitch;

        if (usingAutoYaw)
        {
            Quaternion final = sumSurfaceAdjust * cameraTargetRot * pitch;//使用sumSurfaceRot來計算!
            calculateAutoYawRot(isTriggerYawFollow, final);
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
        // todo 取得輸入
        float Rscale = 0;
        R += Rdiff * Rscale * Time.deltaTime;
        R = Mathf.Max(cameraMinDistance, R);

        Vector3 newPos = new Vector3(0, 0, -R * RScale);

        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, newPos, posFollowSpeed * Time.deltaTime);
        Debug.DrawLine(transform.position, realCamera.position, Color.red);
    }
}