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

public class PlanetMovable : MonoBehaviour {

    GrounGravityGenerator grounGravityGenerator;
    public MonoBehaviour grounGravityGeneratorSocket;

    MoveController moveController;
    public MonoBehaviour moveControllerSocket;
    public Rigidbody rigid;
    public float rotationSpeed = 6f;
    public float gravityScale = 360f;
    public float moveForceScale = 600f;
    public bool useRayHitNormal = false;

    
    Transform m_Cam;
    
    // Use this for initialization
    void Start () {
        m_Cam = Camera.main.transform;

        if (grounGravityGeneratorSocket != null)
            grounGravityGenerator = grounGravityGeneratorSocket as GrounGravityGenerator;

        if (moveControllerSocket != null)
            moveController = moveControllerSocket as MoveController;
    }

    

    public Vector3 getGroundUp()
    {
        return groundUp;
    }

    public bool ladding = false;

    public Vector3 groundUp;
    bool firstUpdate = true;
    // Update is called once per frame
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

            
            //把moveForce投影到地面(期望可以貼著地面移動)
            moveForce = Vector3.ProjectOnPlane(moveForce, adjustRefNormal);
            moveForce.Normalize();

            //更新面向begin
            Vector3 forward2 = moveForce;
            if (forward2 != Vector3.zero)
            {
                Quaternion targetRotation2 = Quaternion.LookRotation(forward2, groundUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
            }
            //更新面向end

            //使用rigid.velocity的話，下面的重力就會失效
            //addForce就可以有疊加的效果
            //雪人的mass也要作相應的調整，不然會推不動骨牌
            rigid.AddForce(moveForceScale * moveForce, ForceMode.Acceleration);
            
        }

        //加上重力
        rigid.AddForce(gravityScale * planetGravity, ForceMode.Acceleration);

        //跳
        if(ladding && moveController.doJump())
            rigid.AddForce(20*gravityScale * -planetGravity, ForceMode.Acceleration);

        //print("rigid="+rigid.velocity.magnitude);

        if(rigid.velocity.magnitude>0.01f)
        Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }
 
}
