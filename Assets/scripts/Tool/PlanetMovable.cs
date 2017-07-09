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

public interface GrounGravityGenerator
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
    static bool findMoveForceMethod1 = true;
    MoveForceMonitor moveForceMonitor;
    public MonoBehaviour moveForceMonitorSocket;

    JumpForceMonitor jumpForceMonitor;
    public MonoBehaviour jumpForceMonitorSocket;

    GrounGravityGenerator grounGravityGenerator;
    public MonoBehaviour grounGravityGeneratorSocket;

    MoveController moveController;
    public MonoBehaviour moveControllerSocket;

    public Rigidbody rigid;
    public float rotationSpeed = 6f;

    static float gravityScale = 92;
    static float gravityScaleOnAir = 40;

    public bool firstPersonMode = false;
    public bool useUserDefinedJumpForce = false;
    static bool useRayHitNormal = true;

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

        if (jumpForceMonitorSocket != null)
            jumpForceMonitor = jumpForceMonitorSocket as JumpForceMonitor;
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
        //groud | block | canJump
        int layerMask = 1 << 10 | 1 << 14 | 1 << 15;
        Vector3 adjustRefNormal = groundUp;
        if (Physics.Raycast(from, -groundUp, out hit, 5, layerMask))
        {
            if (useRayHitNormal)
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

            //在場景plane.untiy還是可以感覺到這2個方法的不同
            if (findMoveForceMethod1)
            {
                //為了修正這個問題
                //https://www.youtube.com/watch?v=8EE8NlZz274
                //不過這麼一來，原本可以順利滑過的case，也變的會卡住了
                if (ladding)
                {
                    //備註：即使是使用PlanetGravityGenerator
                    //OLD和投影到平面之後的moveForce還是可能不一樣
                    //因為PanetMovalbe有可能跑的比Camera快

                    //改成用求2平面的交線(也就是用2個平面的法向量作外積)
                    //其中1個平面就是地面，另一個平面則是和moveForce向量重疊的平面
                    Vector3 normalOfMoveForcePlane = Vector3.Cross(groundUp, moveForce);
                    Vector3 OLD = moveForce;
                    moveForce = Vector3.Cross(normalOfMoveForcePlane, adjustRefNormal);

                    Debug.DrawLine(transform.position, transform.position + adjustRefNormal, Color.black);
                    Debug.DrawLine(transform.position, transform.position + moveForce, Color.blue);
                    Debug.DrawLine(transform.position, transform.position + OLD, Color.red);
                }
                
            }
            else //直接投影到平面上
                moveForce =Vector3.ProjectOnPlane(moveForce, adjustRefNormal);

            //不再正規化moveForce
            //當moveForce和所在平面平行時，力會最大

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
            if(moveForceMonitor!=null)
                rigid.AddForce(moveForceMonitor.getMoveForceStrength(!ladding) * moveForce, ForceMode.Acceleration);
            
        }

        if (animator != null)
        {
            bool moving = rigid.velocity.magnitude > 0.1;
            animator.SetBool("moving", moving);
        }


        //加上重力
        //如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        if (ladding)
            rigid.AddForce(gravityScale * planetGravity, ForceMode.Acceleration);
        else
            rigid.AddForce(gravityScaleOnAir * planetGravity, ForceMode.Acceleration);

        //跳
        if (ladding)
        {
            if (animator != null)
                animator.SetBool("onAir", false);
            Debug.DrawLine(transform.position, transform.position - transform.up,Color.green);
            if (doJump)
            {
                if(jumpForceMonitor!=null)
                    rigid.AddForce(jumpForceMonitor.getJumpForceStrength() * -planetGravity, ForceMode.Acceleration);

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

        debugVelocity = rigid.velocity.magnitude;

        //if (rigid.velocity.magnitude>0.01f)
        //Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }

    void OnCollisionStay(Collision collision)
    {
        upWhenCollision(collision);

        //況著牆移動
        addForceAlongWall(collision);
    }

    void OnCollisionExit(Collision collision)
    {
    }

    void upWhenCollision(Collision collision)
    {
        //只有layer是Block才作
        bool isBlock = collision.gameObject.layer == 14;
        if (!isBlock)
            return;

        if (!ladding)
        { 
            ContactPoint cp = collision.contacts[0];
            Debug.DrawRay(cp.point, 10 * cp.normal, Color.red);

            //screenshot/needUpWhenCollisionSituation.png
            //發現有這2種情況需要addForce，無論那1種都相當於位在圓柱內
            float r =Vector3.ProjectOnPlane(cp.point - transform.position, groundUp).magnitude;
            print(r);
            if (r < 0.6f)//比0.65小一點
            {
                print("翻越");
                rigid.AddForce(getGroundUp() * 5, ForceMode.VelocityChange);
            }
        }
    }

    void addForceAlongWall(Collision collision)
    {
        //在空中不作
        if (!ladding)
            return;

        //只有layer是Block才作
        bool isBlock = collision.gameObject.layer == 14;
        if (!isBlock)
            return;

        ContactPoint cp = collision.contacts[0];

        //cp.point在腳邊不作
        float dotValue = Vector3.Dot(cp.point - transform.position, groundUp);
        if (dotValue < 0.1)
            return;
        Debug.DrawRay(transform.position, 10 * cp.normal, Color.red);

        Vector3 moveForward = transform.forward;
        Vector3 wallNormal = Vector3.ProjectOnPlane(cp.normal, groundUp);

        Debug.DrawRay(transform.position, wallNormal, Color.yellow);
        Vector3 f = Vector3.ProjectOnPlane(moveForward, wallNormal);

        //當moveForward和f接近平行時k值要小
        //當moveForward和f接近垂直時k值要大
        float dot = Vector3.Dot(moveForward, f);
        float k = 1 - dot;
        k = Mathf.Max(k, 0.2f);

        float strength = 2.5f;
        rigid.AddForce(f * k * strength, ForceMode.VelocityChange);
        //rigid.AddForce(f * k * strength, ForceMode.Acceleration);

        float strengthPerpendicularWall = 1;

        rigid.AddForce(wallNormal* strengthPerpendicularWall, ForceMode.VelocityChange);
        Debug.DrawRay(transform.position, 10 * f, Color.blue);
    }
}
