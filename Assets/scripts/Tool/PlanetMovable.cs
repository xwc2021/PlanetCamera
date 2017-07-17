﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public interface InputProxy
{
    Vector2 getHV();
    bool pressJump();
    bool pressFire();
    bool holdFire();
}

public interface GroundGravityGenerator
{
    Vector3 findGroundUp();
}

public interface MoveForceMonitor
{
    float getMoveForceStrength(bool isOnAir);
}

public interface JumpForceMonitor
{
    float getJumpForceStrength();
}



public class PlanetMovable : MonoBehaviour
{
    public SlopeForceMonitor slopeForceMonitor;

    GroundGravityGenerator grounGravityGenerator;
    public GravityDirectionMonitor gravityDirectionMonitor;
    MoveForceMonitor moveForceMonitor;
    public MonoBehaviour moveForceMonitorSocket;

    JumpForceMonitor jumpForceMonitor;
    public MonoBehaviour jumpForceMonitorSocket;
    
    MoveController moveController;
    public MonoBehaviour moveControllerSocket;

    public Rigidbody rigid;
    public float rotationSpeed = 6f;

    static float gravityScale = 92;
    static float gravityScaleOnAir = 40;

    public bool firstPersonMode = false;
    public bool useUserDefinedJumpForce = false;
    public float detectForwardOffset = 0.2f;
    public float backOffset = -0.1f;

    void setAnimatorInfo()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            return;
        beforeJump = animator.GetBehaviour<BeforeJumpState>();
        if (beforeJump == null)
            return;

        beforeJump.setRigid(rigid);
        onAirHash = Animator.StringToHash("Base Layer.onAir");
    }

    int onAirHash;
    Animator animator;
    BeforeJumpState beforeJump;
    // Use this for initialization
    void Start () {

        contactPointList = new List<ContactPoint[]>();
        setAnimatorInfo();

        if (moveControllerSocket != null)
            moveController = moveControllerSocket as MoveController;

        if (moveForceMonitorSocket != null)
            moveForceMonitor = moveForceMonitorSocket as MoveForceMonitor;

        if (jumpForceMonitorSocket != null)
            jumpForceMonitor = jumpForceMonitorSocket as JumpForceMonitor;
    }

    public void ResetGravityGenetrator(GravityGeneratorEnum pggEnum)
    {
        gravityDirectionMonitor.ResetGravityGenerator(pggEnum);
    }

    bool doJump=false;
    private void Update()
    {
        //判定有沒有接觸
        contact = isContact();

        if (moveController == null)
            return;

        //這邊要加上if (!doJump)的判斷，因為：
        //如果在|frame1|按下跳，其實會在|frame2|的Update裡才執行GetButtonDown檢查(在同個Frame裡FixedUpdate會先於Update執行)
        //這時GetButtonDown為true，但要等到|frame3|才會執行到fixedUPdate
        //如果|frame3|裡沒有fixedUpdate，接著還是會執行Update，這時GetButtonDown檢查已經變成false了
        //所以到|frame4|時執行fixedUpdate還是不會跳

        // |frame1| |frame2||frame3||frame4|
        //http://gpnnotes.blogspot.tw/2017/04/blog-post_22.html
        if (!doJump)
            doJump = moveController.doJump();
    }


    public Vector3 getGroundUp()
    {
        return groundUp;
    }

    private void getGroundNormalNow(out Vector3 planeNormal,out bool isHit)
    {
        RaycastHit hit;
        //https://www.youtube.com/watch?v=Cq_Wh8o96sc
        //往後退一步，下斜坡不卡住(因為在交界處有如果直直往下打可能打中斜坡)
        Vector3 from = transform.forward * backOffset + groundUp + transform.position;
        //Debug.DrawRay(from, -groundUp*2 , Color.red);
        isHit = false;
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.Block | 1 << LayerDefined.canJump;
        planeNormal = groundUp;
        if (Physics.Raycast(from, -groundUp, out hit, rayCastDistanceToGround, layerMask))
        {
            Vector3 diff = hit.point - transform.position;
            float height = Vector3.Dot(diff, groundUp);

            planeNormal = hit.normal;

            float distance = diff.magnitude;
            //如果距離小於某個值就判定是在地面上
            if (distance < isHitDistance)
            {
                isHit = true;
            }   
        }
    }

    private void getGroundNormalPredict(out Vector3 planeNormalPredict)
    {
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.Block;
        RaycastHit hit;
        planeNormalPredict = groundUp;
        Vector3 from = transform.forward * detectForwardOffset + groundUp + transform.position;
        //Debug.DrawRay(from, -groundUp * 2, Color.green);
        if (Physics.Raycast(from, -groundUp, out hit, rayCastDistanceToGround, layerMask))
            planeNormalPredict = hit.normal;
    }

    private void getHitWallNormal(out Vector3 wallNormal,out bool isHitWall)
    {
        int layerMask = 1 << LayerDefined.Block;
        RaycastHit hit;
        
        isHitWall = false;
        wallNormal = Vector3.zero;

        float SphereR = 0.24f;
        float forwardToWall=0.5f;
        float leftRightTowall = 0.2f;
        float []rayCastDistanceToWall = { forwardToWall, forwardToWall, forwardToWall, leftRightTowall, leftRightTowall };


        float heightThreshold = 0.1f;
        float[] height = { 0.25f, 0.6f, 1.2f,0.6f,0.6f };
        Vector3[] dir = { transform.forward, transform.forward, transform.forward, transform.right, -transform.right };
        for (int i = 0; i < 5; i++)
        {
            Vector3 from = groundUp * height[i] + transform.position;
            Debug.DrawRay(from, dir[i]* rayCastDistanceToWall[i], Color.green);
            if (Physics.SphereCast(from, SphereR, dir[i], out hit, rayCastDistanceToWall[i], layerMask))
            {
                float h = Vector3.Dot(hit.point - transform.position, groundUp);
                
                //高過才算
                if (h > heightThreshold)
                {
                    isHitWall = true;
                    wallNormal = hit.normal;
                    Debug.DrawRay(hit.point, hit.normal * 2, Color.red);
                    Debug.DrawRay(from, transform.forward, Color.yellow);
                    return;
                }
            }
        }  
    }

    void setNewMoveForceAlongWall(Vector3 wallNormal, ref Vector3 moveForce,out bool isSetNewValueAlongWall)
    {
        bool onAir = !ladding;

        wallNormal = Vector3.ProjectOnPlane(wallNormal, groundUp);
        Debug.DrawRay(transform.position, wallNormal, Color.red);
        float dotValue = Vector3.Dot(moveForce, wallNormal);

        Vector3 newMoveForce = Vector3.ProjectOnPlane(moveForce, wallNormal);
        //如果移動的方向和wallNormal接近垂直，newMoveForce就可能變的很短
        isSetNewValueAlongWall = newMoveForce.magnitude > 0.01f;
        if(isSetNewValueAlongWall)
            moveForce = newMoveForce;
    }

    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    List<ContactPoint[]> contactPointList;
    public bool isSetNewValueAlongWall;
    public bool isHitWall;
    public bool contact;
    public bool isHit=false;
    public bool ladding = false;
    public bool maybeStick = false;
    static float maybeStcikThreshold = -0.05f;
    static float isHitDistance = 0.2f;
    static float rayCastDistanceToGround = 2;
    Vector3 groundUp;
    // Update is called once per frame
    public float debugVelocity;
    Vector3 wallNormal;
    void FixedUpdate()
    {
        contactPointList.Clear();

        //計算重力方向
        grounGravityGenerator = gravityDirectionMonitor.getGravityGenerator();
        groundUp = grounGravityGenerator.findGroundUp();  
        Vector3 gravityDir = -groundUp;

        //設定面向
        Vector3 forward = Vector3.Cross(transform.right, groundUp);
        Quaternion targetRotation = Quaternion.LookRotation(forward, groundUp);
        transform.rotation = targetRotation;

        //判定是否擊中平面
        Vector3 planeNormal;
        getGroundNormalNow(out planeNormal, out isHit);

        //如果只用contact判定，下坡時可能contact為false
        ladding = contact || isHit;
       
        maybeStick = false;
        if (moveController!=null)
        {
            Vector3 moveForce = moveController.getMoveForce();
            //Debug.DrawLine(transform.position, transform.position + moveForce * 10, Color.blue);

            isSetNewValueAlongWall = false;
            //在地面才作
            if (ladding)
            {
                Vector3 moveForceAlongPlane = Vector3.ProjectOnPlane(moveForce, planeNormal);

                //預測斜坡normal
                Vector3 planeNormalPredict;
                getGroundNormalPredict(out planeNormalPredict);

                float dotValue =Vector3.Dot(moveForceAlongPlane, planeNormalPredict);
                //如果斜坡對PlanetMovalbe存在反作用力的話，就順著斜坡移動
                if (dotValue < maybeStcikThreshold)
                {
                    maybeStick = true;
                    moveForce = Vector3.ProjectOnPlane(moveForce, planeNormalPredict);
                }
                else
                    moveForce = moveForceAlongPlane;

                
                getHitWallNormal(out wallNormal, out isHitWall);

                if (isHitWall)
                    setNewMoveForceAlongWall(wallNormal, ref moveForce,out isSetNewValueAlongWall);

                moveForce.Normalize();
                Debug.DrawRay(transform.position+transform.up, moveForce*5, Color.blue);
            }

            //更新面向begin
            Vector3 forward2 = moveForce;
            if (forward2 != Vector3.zero && !firstPersonMode)
            {
                Quaternion targetRotation2 = Quaternion.LookRotation(forward2, groundUp);
                Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
                transform.rotation = newRot;
            }
            //更新面向end

            //使用rigid.velocity的話，下面的重力就會失效
            //addForce就可以有疊加的效果
            //雪人的mass也要作相應的調整，不然會推不動骨牌
            if (moveForceMonitor != null)
            {
                float moveForceStrength = moveForceMonitor.getMoveForceStrength(!ladding) ;
                Vector3 moveForceWithStrength = moveForceStrength * moveForce;
                if (slopeForceMonitor != null && ladding)
                {
                        moveForceWithStrength=slopeForceMonitor.modifyMoveForce(moveForce, moveForceStrength, getGravityForceStrength(), groundUp, planeNormal);
                }

                rigid.AddForce(moveForceWithStrength, ForceMode.Acceleration);
            }
        }

        if (animator != null)
        {
            bool moving = rigid.velocity.magnitude > 0.05;
            animator.SetBool("moving", moving);
        }

        //加上重力
        //如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        rigid.AddForce(getGravityForceStrength() * gravityDir, ForceMode.Acceleration);

        //幫忙推一把
        if(maybeStick)
            rigid.AddForce(-gravityDir, ForceMode.VelocityChange);

        float strength = 0.5f;
        if(isHitWall && isSetNewValueAlongWall)
            rigid.AddForce(wallNormal* strength, ForceMode.VelocityChange);

        //跳
        if (ladding)
        {
            if (animator != null)
            {
                bool isOnAir = onAirHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (isOnAir)
                    animator.SetBool("onAir", false);
            } 

            Debug.DrawLine(transform.position, transform.position - transform.up,Color.green);
            if (doJump)
            {
                if (jumpForceMonitor != null)
                {
                    if(beforeJump!=null)
                        beforeJump.setAcceleration(jumpForceMonitor.getJumpForceStrength() * -gravityDir);
                }
                    
                if (animator != null)
                    animator.SetBool("beforeJump", true);

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

        //print("rigid="+rigid.velocity.magnitude);

        debugVelocity = rigid.velocity.magnitude;

        //if (rigid.velocity.magnitude>0.01f)
        //Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }

    float getGravityForceStrength()
    {
        if (ladding)
            return gravityScale;
        else
           return gravityScaleOnAir;
    }

    void OnCollisionStay(Collision collision)
    {
        bool isGround = collision.gameObject.layer == LayerDefined.ground;
        bool isBlock = collision.gameObject.layer == LayerDefined.Block;
        bool isCanJump = collision.gameObject.layer == LayerDefined.canJump;

        if (isGround || isBlock || isCanJump)
        {
            //有可能同時碰到2個以上的物件，所以先收集起來
            contactPointList.Add(collision.contacts);
        }
    }

    bool isContact()
    {
        int listCount = contactPointList.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointList[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Vector3 diif = cp[i].point - transform.position;
                float height = Vector3.Dot(groundUp, diif);
                if (height < 0.15f)
                {
                    return true;
                }
            }

        }
        return false;
    }
}
