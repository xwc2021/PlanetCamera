using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

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
    SurfaceFollowHelper surfaceFollowHelper;
    MultiplayerCameraManager multiplayerCameraManager;
    public PostProcessingBehaviour postProcessingBehaviour;
    public GameObject canvas;
    public GameObject eventSystem;
    public MeasuringJumpHeight measuringJumpHeight;
    public PlanetMovable planetMovable;

    public bool adjustCameraWhenMove = true;
    InputProxy inputProxy;
    public Transform m_Cam;
    Rigidbody rigid;
    Animator animator;
    int onAirHash;
    
    bool doJump = false;
    // Use this for initialization
    void Awake()
    {
        surfaceFollowHelper = GetComponent<SurfaceFollowHelper>();
        multiplayerCameraManager = GetComponent<MultiplayerCameraManager>();

        //print("cameraBehavior="+cameraBehavior);

#if (UNITY_ANDROID)
        AndroidInput androidInput = GetComponent<AndroidInput>();
        inputProxy = androidInput as InputProxy;
        postProcessingBehaviour.enabled = false;
#else
        PCInput pcInput = GetComponent<PCInput>();
        inputProxy = pcInput as InputProxy;
#endif
        Debug.Assert(inputProxy != null);
        if (inputProxy.enableControlUI())
        {
            canvas.SetActive(true);
            eventSystem.SetActive(true);
        }

        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        onAirHash = Animator.StringToHash("Base Layer.onAir");

        getCamera();
    }

    public InputProxy getInputProxy()
    {
        return inputProxy;
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

    void setAnimatorMoving()
    {
        bool moving = rigid.velocity.magnitude > 0.05;
        animator.SetBool("moving", moving);
    }

    void setAnimatorDoJump()
    {
        animator.SetBool("doJump", true);
    }

    void setAnimatorOnAir()
    {
        animator.SetBool("onAir", false);
    }

    void syncAnimatorAndRot()
    {
        if (multiplayerCameraManager != null)
        {
            bool moving = animator.GetBool("moving");
            bool doJump =animator.GetBool("doJump");
            bool onAir = animator.GetBool("onAir");
            multiplayerCameraManager.CmdSyncAnimatorAndRot(moving, doJump, onAir, transform.rotation);
        }
    }

    void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.setupRequireData();

        planetMovable.executeGravityForce();
        planetMovable.executeMoving();

        setAnimatorMoving();

        processWallJump();
        processJump();
        processLadding();

        syncAnimatorAndRot();

        //從Update移到FixedUpdate
        //因為無法保證FixedUpdate在第1個frame一定會執行到
        if (surfaceFollowHelper!=null)
            surfaceFollowHelper.doAdjustByGroundUp();
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

                setAnimatorDoJump();

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
                    setAnimatorOnAir();
                    if (measuringJumpHeight != null)
                        measuringJumpHeight.stopRecord();
                }
            }
        }
    }


    void Update()
    {
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

    //https://msdn.microsoft.com/zh-tw/library/14akc2c7.aspx
    void doDegreeLock(ref float h, ref float v)
    {
        //16個方向移動
        int lockPiece = 16;
        float snapDegree = 360.0f / lockPiece;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(v, h);

        float extraDegree = degree % snapDegree;

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
        hv.Normalize();
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