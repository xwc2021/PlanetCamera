using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public interface InputProxy
{
    Vector2 getHV();
    bool pressJump();
}

public interface GrounGravityGenerator
{
    Vector3 findGroundUp();
}

public interface MoveForceMonitor
{
    float getNowForceStrength();
}

public class PlanetMovable : MonoBehaviour
{
    MoveForceMonitor moveForceMonitor;
    public MonoBehaviour moveForceMonitorSocket;

    GrounGravityGenerator grounGravityGenerator;
    public MonoBehaviour grounGravityGeneratorSocket;

    MoveController moveController;
    public MonoBehaviour moveControllerSocket;
    public Rigidbody rigid;
    public float rotationSpeed = 6f;
    public float gravityScale = 100f;
    public bool useRayHitNormal = false;
    public bool firstPersonMode = false;
    public float jumpForce= 100f;
    public bool useUserDefinedJumpForce = false;

    Animator animator;
    // Use this for initialization
    void Start () {

        animator = GetComponentInChildren<Animator>();

        if (grounGravityGeneratorSocket != null)
            grounGravityGenerator = grounGravityGeneratorSocket as GrounGravityGenerator;

        if (moveControllerSocket != null)
            moveController = moveControllerSocket as MoveController;

        if (moveForceMonitorSocket != null)
            moveForceMonitor = moveForceMonitorSocket as MoveForceMonitor;
    }

    bool doJump=false;
    private void Update()
    {
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

    bool ladding = false;

    Vector3 groundUp;
    // Update is called once per frame
    float velocity;
    void FixedUpdate()
    {
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
        int layerMask = 1 << 10;
        Vector3 adjustRefNormal = groundUp;
        if (Physics.Raycast(from, -groundUp, out hit, 5, layerMask))
        {
            if (useRayHitNormal)
                adjustRefNormal = hit.normal;

                float distance = (hit.point - transform.position).magnitude;
            //如果距離小於某個值就判定是在地面上
            if (distance < 0.5f)
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
            rigid.AddForce(moveForceMonitor.getNowForceStrength() * moveForce, ForceMode.Acceleration);
            
        }

        if (animator != null)
        {
            bool moving = rigid.velocity.magnitude > 2;
            animator.SetBool("moving", moving);
        }
        

        //加上重力
        rigid.AddForce(gravityScale * planetGravity, ForceMode.Acceleration);

        //跳
        if (ladding)
        {
            if (animator != null)
                animator.SetBool("onAir", false);
            Debug.DrawLine(transform.position, transform.position - transform.up,Color.green);
            if (doJump)
            {
                if (useUserDefinedJumpForce)
                    rigid.AddForce(jumpForce * -planetGravity, ForceMode.Acceleration);
                else
                    rigid.AddForce(20 * gravityScale * -planetGravity, ForceMode.Acceleration);

                if (animator != null)
                    animator.SetBool("onAir", true);

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

        velocity = rigid.velocity.magnitude;

        //if (rigid.velocity.magnitude>0.01f)
        //Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }
}
