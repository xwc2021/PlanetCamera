using UnityEngine;
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
    //這2個數值就是用MasuringJumpHeight量出來的QQ
    public float normalHeight = 3.350325f;
    public float turboHeight = 4.04285f;

    public MeasuringJumpHeight measuringJumpHeight;
    public AvoidStickTool avoidStickTool;
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

    static float gravityScale = 10;
    static float gravityScaleOnAir = 40;

    public bool firstPersonMode = false;
    public bool useUserDefinedJumpForce = false;
    
    public float backOffset = -0.1f;

    int onAirHash;
    Animator animator;

    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    List<ContactPoint[]> contactPointGround;
    List<ContactPoint[]> contactPointWall;
    public bool contactGround;
    public bool touchWall;
    public bool isHit = false;
    public bool ladding = false;
    public bool isReachTargetHeight = true;
    public bool isWaitToTargetHeight = false;

    static float isHitDistance = 0.2f;
    public static float rayCastDistanceToGround = 2;
    Vector3 groundUp;
    // Update is called once per frame
    public float debugVelocity;
    Vector3 wallNormal;

    void setAnimatorInfo()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            return;

        onAirHash = Animator.StringToHash("Base Layer.onAir");
    }

    // Use this for initialization
    void Start () {

        contactPointGround = new List<ContactPoint[]>();
        contactPointWall= new List<ContactPoint[]>();
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
        contactGround = isContactGround();
        touchWall =isTouchWall();

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
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.groundNotBlockCamera;
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

    void FixedUpdate()
    {
        //清空
        contactPointGround.Clear();
        contactPointWall.Clear();

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
        ladding = contactGround || isHit;

        processMove(planeNormal, gravityDir);

        if (animator != null)
        {
            bool moving = rigid.velocity.magnitude > 0.05;
            animator.SetBool("moving", moving);
        }

        /*
        testReachTargetHeight();

        isWaitToTargetHeight = waitToTargetHeight(gravityDir);
        if (isWaitToTargetHeight) 
            return;
        */

        //加上重力
        //如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        rigid.AddForce(getGravityForceStrength() * gravityDir, ForceMode.Acceleration);

        processJump(gravityDir);
        processLadding();

        debugVelocity = rigid.velocity.magnitude;
    }

    void testReachTargetHeight()
    {
        if (!isReachTargetHeight)
        {
            if (!ladding)
            {
                bool isReach = Mathf.Abs(measuringJumpHeight.height - normalHeight) < 0.01f;
                bool over = measuringJumpHeight.height > normalHeight;
                isReachTargetHeight = isReach || over;
            }
        }
    }

    bool waitToTargetHeight(Vector3 gravityDir)
    {
        if (!ladding && touchWall)
        {
            if (!isReachTargetHeight)
            {
                float diff =normalHeight - measuringJumpHeight.height;
                float speed = diff>0.1f?5:100;
                Vector3 targetPos = transform.position+transform.up * (diff);
                transform.position = Vector3.Lerp(transform.position, targetPos, speed*Time.fixedDeltaTime);

                return true;
            }
        }

        return false;
    }

    void processMove(Vector3 planeNormal,Vector3 gravityDir)
    {
        if (moveController != null)
        {
            Vector3 moveForce = moveController.getMoveForce();
            //Debug.DrawLine(transform.position, transform.position + moveForce * 10, Color.blue);

            //在地面才作
            if (ladding)
            {
                moveForce = Vector3.ProjectOnPlane(moveForce, planeNormal);

                if (avoidStickTool != null)
                {
                    avoidStickTool.alongSlopeOrGround(ref moveForce, planeNormal, gravityDir);
                }

                moveForce.Normalize();
                Debug.DrawRay(transform.position + transform.up, moveForce * 5, Color.blue);
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

            //addForce就可以有疊加的效果
            //雪人的mass也要作相應的調整，不然會推不動骨牌
            if (moveForceMonitor != null)
            {
                float moveForceStrength = moveForceMonitor.getMoveForceStrength(!ladding);
                Vector3 moveForceWithStrength = moveForceStrength * moveForce;
                if (slopeForceMonitor != null && ladding)
                {
                    moveForceWithStrength = slopeForceMonitor.modifyMoveForce(moveForce, moveForceStrength, getGravityForceStrength(), groundUp, planeNormal);
                }

                rigid.AddForce(moveForceWithStrength, ForceMode.Acceleration);
            }
        }
    }

    void processLadding()
    {
        if (ladding)
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

    private void processJump(Vector3 gravityDir)
    {
        //jump from wall
        if (!ladding && touchWall)
        {
            if (doJump)
            {
                if (jumpForceMonitor != null)
                {
                    rigid.AddForce(jumpForceMonitor.getJumpForceStrength() * -gravityDir, ForceMode.Acceleration);
                    float s = 20;
                    rigid.AddForce( s * touchWallNormal, ForceMode.VelocityChange);
                }
                doJump = false;
            }
        }

        //跳
        if (ladding)
        {
            Debug.DrawLine(transform.position, transform.position - transform.up, Color.green);
            if (doJump)
            {
                if (jumpForceMonitor != null)
                {
                    if (measuringJumpHeight != null)
                        measuringJumpHeight.startRecord();

                    isReachTargetHeight = false;
                    rigid.AddForce(jumpForceMonitor.getJumpForceStrength() * -gravityDir, ForceMode.Acceleration);
                }

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

    float getGravityForceStrength()
    {
        if (ladding)
            return gravityScale;
        else
           return gravityScaleOnAir;
    }

    void OnCollisionStay(Collision collision)
    {
        bool ground = collision.gameObject.layer == LayerDefined.ground;
        bool groundNotBlockCamera = collision.gameObject.layer == LayerDefined.groundNotBlockCamera;

        if (ground || groundNotBlockCamera)
        {
            //有可能同時碰到2個以上的物件，所以先收集起來
            contactPointGround.Add(collision.contacts);
        }

        bool wall = collision.gameObject.layer == LayerDefined.wall;
        bool wallNotBlockCamera = collision.gameObject.layer == LayerDefined.wallNotBlockCamera;
        if (wall | wallNotBlockCamera)
        {
            //有可能同時碰到2個以上的物件，所以先收集起來
            contactPointWall.Add(collision.contacts);
        }
    }

    bool isContactGround()
    {
        int listCount = contactPointGround.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointGround[x];
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

    Vector3 touchWallNormal;
    bool isTouchWall()
    {
        int listCount = contactPointWall.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointWall[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Vector3 diif = cp[i].point - transform.position;
                float height = Vector3.Dot(groundUp, diif);
                if (height > 0.15f && height < 1.0f)
                {
                    touchWallNormal = cp[i].normal;
                    return true;
                }
            }

        }
        return false;
    }

    public void enableNormalRigid()
    {
        rigid.drag =4;
    }

    public void enableIceSkatingRigid()
    {
        rigid.drag = 0.5f;
    }
}
