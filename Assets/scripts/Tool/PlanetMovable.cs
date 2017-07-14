using UnityEngine;
using System.Collections;
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

    public bool ladding = false;

    Vector3 groundUp;
    // Update is called once per frame
    public float debugVelocity;
    void FixedUpdate()
    {
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
        Vector3 from = groundUp + transform.position;

        ladding = false;
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.Block | 1 << LayerDefined.canJump;
        Vector3 adjustRefNormal = groundUp;
        if (Physics.Raycast(from, -groundUp, out hit, 5, layerMask))
        {
                adjustRefNormal = hit.normal;

            float distance = (hit.point - transform.position).magnitude;
            //如果距離小於某個值就判定是在地面上
            if (distance < 0.1f)
            {
                ladding = true;
                //print("ladding");
            }
            //else print("float");    
        }
   
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
                //moveForce = Vector3.Cross(normalOfMoveForcePlane, adjustRefNormal);

                moveForce=Vector3.ProjectOnPlane(moveForce, adjustRefNormal);
                moveForce.Normalize();
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
                        moveForceWithStrength=slopeForceMonitor.modifyMoveForce(moveForce, moveForceStrength, getGravityForceStrength(), groundUp, adjustRefNormal);
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
}
