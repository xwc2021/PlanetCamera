using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SurfaceFollowCameraBehavior
{
    void setSurfaceRotate(bool doRotateFollow, Quaternion adjustRot);
}

public interface MoveController
{
    Vector3 getMoveForce();
    bool doJump();
}

public class PlanetPlayerController : MonoBehaviour, MoveController
{
    public PlanetMovable planetMovable;
    SurfaceFollowCameraBehavior followCameraBehavior;
    public MonoBehaviour followCameraBehaviorSocket;
    public bool adjustCameraWhenMove = true;
    InputProxy inputProxy;
    public MonoBehaviour inputPorxySocket;
    public Transform m_Cam;

    Vector3 previousPosistion;
    Vector3 previousGroundUp;

    // Use this for initialization
    void Awake()
    {
        previousPosistion = transform.position;
        previousGroundUp = transform.up;

        if (followCameraBehaviorSocket != null)
            followCameraBehavior = followCameraBehaviorSocket as SurfaceFollowCameraBehavior;

        //print("cameraBehavior="+cameraBehavior);

        if (inputPorxySocket != null)
            inputProxy = inputPorxySocket as InputProxy;

        getCamera();
    }

    public void getCamera()
    {
        if (m_Cam == null)
        {
            Camera c = GetComponentInChildren<Camera>();
            //在Multiplayer模式可能取不到，因為被disable掉了
            m_Cam = c != null ? c.transform : null;
        }
    }


    void Update()
    {
        if (adjustCameraWhenMove)
            doAdjustByGroundUp();

        Vector2 hv = inputProxy.getHV();
        //followCameraBehavior.adjustCameraYaw(hv.x);
    }

    void doAdjustByGroundUp()
    {
        if (followCameraBehavior == null)
            return;

        //如果位置有更新，就更新FlowPoint
        //透過groundUp和向量(nowPosition-previouPosistion)的外積，找出旋轉軸Z

        Vector3 groundUp = planetMovable.getGroundUp();

        Vector3 Z = Vector3.Cross(previousGroundUp, groundUp);
        //Debug.DrawLine(transform.position, transform.position + Z * 16, Color.blue);
        //Debug.DrawLine(transform.position, transform.position + previousGroundUp * 16, Color.red);
        //Debug.DrawLine(transform.position, transform.position + groundUp * 16, Color.green);

        //算出2個frame之間在planet上移動的角度差
        float cosValue = Vector3.Dot(previousGroundUp, groundUp);

        //http://answers.unity3d.com/questions/778626/mathfacos-1-return-nan.html
        //上面說Dot有可能會>1或<-1
        cosValue = Mathf.Max(-1.0f, cosValue);
        cosValue = Mathf.Min(1.0f, cosValue);

        float rotDegree = Mathf.Acos(cosValue) * Mathf.Rad2Deg;
        //print("rotDegree=" + rotDegree);

        if (float.IsNaN(rotDegree))
        {
            print("IsNaN");
            return;
        }

        float threshold = 0.1f;
        if (rotDegree > threshold)
        {
            //print("rotDegree=" + rotDegree);
            Quaternion q = Quaternion.AngleAxis(rotDegree, Z);

            followCameraBehavior.setSurfaceRotate(true, q);
            previousGroundUp = groundUp;//有轉動才更新
        }
    }

    //https://msdn.microsoft.com/zh-tw/library/14akc2c7.aspx
    void doDegreeLock(ref float h, ref float v)
    {
        //16個方向移動
        int lockPiece = 16;
        float snapDegree = 360.0f / lockPiece;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(v, h);

        float extraDegree = degree % snapDegree;
        float finalDegree = degree - extraDegree;

        float extraRad = extraDegree * Mathf.Deg2Rad;

        //作旋轉修正
        float newH = h * Mathf.Cos(-extraRad) + v * -Mathf.Sin(-extraRad);
        float newV = h * Mathf.Sin(-extraRad) + v * Mathf.Cos(-extraRad);

        h = newH;
        v = newV;
    }

    public bool doDergeeLock = false;
    public Vector3 getMoveForce()
    {
        //取得輸入
        Vector2 hv = inputProxy.getHV();
        float h = hv.x;
        float v = hv.y;

        if (h != 0 || v != 0)
        {
            if(m_Cam==null)
                return Vector3.zero;

            Vector3 moveForword = Vector3.Cross(m_Cam.right, transform.up);
            Debug.DrawLine(transform.position, transform.position + moveForword, Color.yellow);
            Vector3 controllForce = h * m_Cam.right + v * moveForword;
            return controllForce.normalized;
        }

        return Vector3.zero;
    }

    public bool doJump()
    {
        return inputProxy.pressJump();
    }

    public bool holdFire()
    {
        return inputProxy.holdFire();
    }

    public bool pressFire()
    {
        return inputProxy.pressFire();
    }
}