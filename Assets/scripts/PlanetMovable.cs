using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;



public interface InputProxy
{
    Vector2 getHV();
    bool pressJump();
}


public class PlanetMovable : MonoBehaviour {

    MoveController moveController;
    public MonoBehaviour moveControllerSocket;
    public Transform laddingPlanet;
    public Rigidbody rigid;
    public float rotationSpeed = 6f;
    public float gravityScale = 360f;
    public float moveForceScale = 600f;
    Transform m_Cam;
    

    // Use this for initialization
    void Start () {
        m_Cam = Camera.main.transform;

        if (moveControllerSocket != null)
            moveController = moveControllerSocket as MoveController;
    }

    public bool ladding = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        //計算重力方向
        Vector3 planetGravity = laddingPlanet.position - transform.position;
        planetGravity.Normalize();

        //設定面向
        Vector3 headUp = -planetGravity;
        Vector3 forward = Vector3.Cross(transform.right, headUp);
        Quaternion targetRotation = Quaternion.LookRotation(forward, headUp);
        transform.rotation = targetRotation;


        //找出地面Normal
        RaycastHit hit;
        Vector3 hitNormal = transform.up;
        Vector3 from = headUp + transform.position;

        ladding = false;
        //原本沒作mask會射到雪人自己，所以就飛起來了
        int layerMask = 1 << 10;
        if (Physics.Raycast(from, -headUp, out hit, 5, layerMask))
        {
            hitNormal = hit.normal;

            float distance = (hit.point - transform.position).magnitude;

            if (distance < 1)
            {
                ladding = true;
                //print("ladding");
            }
            //else print("float");
                
        }

        //如果距離小於某個值就判定是在地面上


        if (moveController!=null)
        {
            Vector3 controllForce = moveController.getMoveForce();

            //把controllForce投影到地面(期望可以貼著地面移動)
            controllForce = Vector3.ProjectOnPlane(controllForce, hitNormal);
            controllForce.Normalize();

            //更新面向begin
            Vector3 forward2 = controllForce;
            if (forward2 != Vector3.zero)
            {
                Quaternion targetRotation2 = Quaternion.LookRotation(forward2, headUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
            }
            //更新面向end

            //使用rigid.velocity的話，下面的重力就會失效
            //addForce就可以有疊加的效果
            //雪人的mass也要作相應的調整，不然會推不動骨牌
            rigid.AddForce(moveForceScale * controllForce,ForceMode.Acceleration);
            
        }

        //加上重力
        rigid.AddForce(gravityScale * planetGravity, ForceMode.Acceleration);

        //跳
        if(ladding && moveController.doJump())
            rigid.AddForce(20*gravityScale * -planetGravity, ForceMode.Acceleration);

        print("rigid="+rigid.velocity.magnitude);

        Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }
 
}
