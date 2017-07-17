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

    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    List<ContactPoint[]> contactPointList;
    public bool contact;
    public bool isHit=false;
    public bool ladding = false;
    public bool maybeStick = false;
    static float maybeStcikThreshold = -0.05f;
    static float isHitDistance = 0.2f;
    static float rayCastDistance = 2;
    Vector3 groundUp;
    // Update is called once per frame
    public float debugVelocity;
    void FixedUpdate()
    {
        contactPointList.Clear();

        grounGravityGenerator = gravityDirectionMonitor.getGravityGenerator();
        groundUp = grounGravityGenerator.findGroundUp();  

        //計算重力方向
        Vector3 planetGravity = -groundUp;

        //設定面向
        Vector3 forward = Vector3.Cross(transform.right, groundUp);
        Quaternion targetRotation = Quaternion.LookRotation(forward, groundUp);
        transform.rotation = targetRotation;

        //判定是否在地面上(或浮空)
        RaycastHit hit;
        //https://www.youtube.com/watch?v=Cq_Wh8o96sc
        //往後退一步，下斜坡不卡住(因為在交界處有可能紅色射線已經打中斜坡)
        Vector3 from = transform.forward * backOffset + groundUp + transform.position;
        Debug.DrawRay(from, -groundUp*2 , Color.red);
        isHit = false;
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.Block | 1 << LayerDefined.canJump;
        Vector3 planeNormal = groundUp;
        if (Physics.Raycast(from, -groundUp, out hit, rayCastDistance, layerMask))
        {
            Vector3 diff = hit.point - transform.position;
            float height = Vector3.Dot(diff, groundUp);

            planeNormal = hit.normal;

            float distance = diff.magnitude;
            //如果距離小於某個值就判定是在地面上
            if (distance < isHitDistance)
            {
                isHit = true;
                //print("ladding");
            }
            //else print("float");    
        }

        //如果只用contact判定，下坡時可能contact為false
        ladding = contact || isHit;

        //預測斜坡normal
        maybeStick = false;
        Vector3 planeNormalPredict = groundUp;
        from = transform.forward* detectForwardOffset + groundUp + transform.position;
        Debug.DrawRay(from, -groundUp * 2, Color.green);
        if (Physics.Raycast(from, -groundUp, out hit, rayCastDistance, layerMask))
            planeNormalPredict = hit.normal;
        //Debug.DrawRay(hit.point, planeNormalPredict * 5, Color.yellow);
        if (moveController!=null)
        {
            Vector3 moveForce = moveController.getMoveForce();
            //Debug.DrawLine(transform.position, transform.position + moveForce * 10, Color.blue);


            //為了修正這個問題
            //https://www.youtube.com/watch?v=8EE8NlZz274
            //不過這麼一來，原本可以順利滑過的case，也變的會卡住了
            if (ladding)
            {

                //2平面的交線(個平面的法向量作外積)
                //Vector3 normalOfMoveForcePlane = Vector3.Cross(groundUp, moveForce);
                //moveForce = Vector3.Cross(normalOfMoveForcePlane, planeNormal);

                Vector3 moveForceAlongPlane = Vector3.ProjectOnPlane(moveForce, planeNormal);

                float dotValue =Vector3.Dot(moveForceAlongPlane, planeNormalPredict);
                //print(dotValue);
                if (dotValue < maybeStcikThreshold)
                {
                    maybeStick = true;
                    moveForce = Vector3.ProjectOnPlane(moveForce, planeNormalPredict);
                }
                else
                    moveForce = moveForceAlongPlane;

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
        rigid.AddForce(getGravityForceStrength() * planetGravity, ForceMode.Acceleration);

        //幫忙推一把
        if(maybeStick)
            rigid.AddForce(-planetGravity, ForceMode.VelocityChange);
        
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
                        beforeJump.setAcceleration(jumpForceMonitor.getJumpForceStrength() * -planetGravity);
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
