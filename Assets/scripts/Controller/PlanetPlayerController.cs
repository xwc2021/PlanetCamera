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
    bool doTurbo();
}

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlanetPlayerController : MonoBehaviour, MoveController
{
    public MeasuringJumpHeight measuringJumpHeight;
    public PlanetMovable planetMovable;
    SurfaceFollowCameraBehavior followCameraBehavior;
    public MonoBehaviour followCameraBehaviorSocket;
    public bool adjustCameraWhenMove = true;
    InputProxy inputProxy;
    public MonoBehaviour inputPorxySocket;
    public Transform m_Cam;
    Rigidbody rigid;
    Animator animator;
    int onAirHash;

    Vector3 previousGroundUp;
    bool doJump = false;
    // Use this for initialization
    void Awake()
    {
        previousGroundUp = transform.up;

        if (followCameraBehaviorSocket != null)
            followCameraBehavior = followCameraBehaviorSocket as SurfaceFollowCameraBehavior;

        //print("cameraBehavior="+cameraBehavior);

        if (inputPorxySocket != null)
            inputProxy = inputPorxySocket as InputProxy;
 
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        onAirHash = Animator.StringToHash("Base Layer.onAir");

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

    void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.setupRequireData();

        planetMovable.executeGravityForce();
        planetMovable.executeMoving();

        bool moving = rigid.velocity.magnitude > 0.05;
        animator.SetBool("moving", moving);

        processWallJump();
        processJump();
        processLadding();
    }

    void processWallJump()
    {
        //jump from wall
        if (!planetMovable.Ladding && planetMovable.TouchWall)
        {
            if (doJump)
            {
                planetMovable.executeJump();
                float s = 20;
                rigid.AddForce(s * planetMovable.TouchWallNormal, ForceMode.VelocityChange);
                doJump = false;
            }
        }
    }

    void processJump()
    {
        //跳
        if (planetMovable.Ladding)
        {
            Debug.DrawLine(transform.position, transform.position - transform.up, Color.green);
            if (doJump)
            {
                if (measuringJumpHeight != null)
                    measuringJumpHeight.startRecord();

                planetMovable.executeJump();

                if (animator != null)
                    animator.SetBool("doJump", true);

                doJump = false;
            }
        }
        else
        {
            //不是ladding時按下doJump，也要把doJump設為false
            //不然的話會一直持續到當ladding為true再進行跳躍
            if (doJump)
                doJump = false;
        }
    }

    void processLadding()
    {
        if (planetMovable.Ladding)
        {
            if (animator != null)
            {
                bool isOnAir = onAirHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (isOnAir)
                {
                    animator.SetBool("onAir", false);
                    if (measuringJumpHeight != null)
                        measuringJumpHeight.stopRecord();
                }
            }
        }
    }


    void Update()
    {
        if (adjustCameraWhenMove)
            doAdjustByGroundUp();

        //這邊要加上if (!doJump)的判斷，因為：
        //如果在|frame1|按下跳，其實會在|frame2|的Update裡才執行GetButtonDown檢查(在同個Frame裡FixedUpdate會先於Update執行)
        //這時GetButtonDown為true，但要等到|frame3|才會執行到fixedUPdate
        //如果|frame3|裡沒有fixedUpdate，接著還是會執行Update，這時GetButtonDown檢查已經變成false了
        //所以到|frame4|時執行fixedUpdate還是不會跳

        // |frame1| |frame2||frame3||frame4|
        //http://gpnnotes.blogspot.tw/2017/04/blog-post_22.html
        if (!doJump)
            doJump = inputProxy.pressJump();
    }

    void doAdjustByGroundUp()
    {
        if (followCameraBehavior == null)
            return;

        //如果位置有更新，就更新FlowPoint
        //透過groundUp和向量(nowPosition-previouPosistion)的外積，找出旋轉軸Z

        Vector3 groundUp = planetMovable.GroundUp;

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
    Vector3 MoveController.getMoveForce()
    {
        //取得輸入
        Vector2 hv = inputProxy.getHV();
        float h = hv.x;
        float v = hv.y;

        if (h != 0 || v != 0)
        {
            if(m_Cam==null)
                return Vector3.zero;

            if (doDergeeLock)
                doDegreeLock(ref h, ref v);

            Vector3 moveForword = Vector3.Cross(m_Cam.right, transform.up);
            Vector3 controllForce = h * m_Cam.right + v * moveForword;
            return controllForce.normalized;
        }

        return Vector3.zero;
    }

    bool MoveController.doTurbo()
    {
        return inputProxy.holdFire();
    }
}