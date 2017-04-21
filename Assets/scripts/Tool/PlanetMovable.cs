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


    // Use this for initialization
    void Start () {

        if (grounGravityGeneratorSocket != null)
            grounGravityGenerator = grounGravityGeneratorSocket as GrounGravityGenerator;

        if (moveControllerSocket != null)
            moveController = moveControllerSocket as MoveController;

        if (moveForceMonitorSocket != null)
            moveForceMonitor = moveForceMonitorSocket as MoveForceMonitor;
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
            if (distance < 1)
            {
                ladding = true;
                //print("ladding");
            }
            //else print("float");    
        }
   
        if (moveController!=null)
        {
            Vector3 moveForce = moveController.getMoveForce();

            //原先的方法：
            //把moveForce投影到地面(期望可以貼著地面移動)
            //moveForce = Vector3.ProjectOnPlane(moveForce, adjustRefNormal);

            //改成用求2平面的交線(也就是用2個平面的法向量作外積)
            //其中1個平面就是地面，另一個平面則是和moveForce向量重疊的平面
            Vector3 normalOfMoveForcePlane = Vector3.Cross(groundUp, moveForce);
            moveForce = Vector3.Cross(normalOfMoveForcePlane, adjustRefNormal);

            moveForce.Normalize();
            //Debug.DrawLine(transform.position, transform.position + moveForce * 10, Color.blue);

            //更新面向begin
            Vector3 forward2 = moveForce;
            if (forward2 != Vector3.zero && !firstPersonMode)
            {
                Quaternion targetRotation2 = Quaternion.LookRotation(forward2, groundUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
            }
            //更新面向end

            //使用rigid.velocity的話，下面的重力就會失效
            //addForce就可以有疊加的效果
            //雪人的mass也要作相應的調整，不然會推不動骨牌
            rigid.AddForce(moveForceMonitor.getNowForceStrength() * moveForce, ForceMode.Acceleration);
            
        }

        //加上重力
        rigid.AddForce(gravityScale * planetGravity, ForceMode.Acceleration);

        //跳
        if (moveController.doJump())
        {
            if (ladding)
                rigid.AddForce(20 * gravityScale * -planetGravity, ForceMode.Acceleration);
            else
                print("起跳失敗");
        }
            

        //print("rigid="+rigid.velocity.magnitude);

        velocity = rigid.velocity.magnitude;

        //if (rigid.velocity.magnitude>0.01f)
        //Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }
}
