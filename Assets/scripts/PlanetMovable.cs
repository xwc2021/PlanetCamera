using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public interface InputProxy
{
    Vector2 getHV();
    bool pressJump();
}

public class PlanetMovable : MonoBehaviour {

    public bool averageSG = true;
    MoveController moveController;
    public MonoBehaviour moveControllerSocket;
    public Rigidbody rigid;
    public float rotationSpeed = 6f;
    public float gravityScale = 360f;
    public float moveForceScale = 600f;
    public float findingGravitySensorR = 4;
    public Transform findingGravitySphere;
    Transform m_Cam;
    
    // Use this for initialization
    void Start () {
        m_Cam = Camera.main.transform;

        if (moveControllerSocket != null)
            moveController = moveControllerSocket as MoveController;

        findingGravitySphere.localScale = new Vector3(findingGravitySensorR, findingGravitySensorR, findingGravitySensorR)*2;

    }

    Collider[] gs = new Collider[100];//大小看需求自己設定
    Vector3 findGroundUp()
    {
        int layerMask = 1 << 11;
        int overlapCount =Physics.OverlapSphereNonAlloc(transform.position, findingGravitySensorR, gs, layerMask);
        //Debug.DrawLine(transform.position, transform.position + groundUp * findingGravitySensorR);

        //print("overlapCount=" + overlapCount);

        if(overlapCount==0)
            return transform.up;

        if (averageSG)
        {
            //找出nearestGS的平均值
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < overlapCount; i++)
            {
                Collider nowGS = gs[i];
                sum = sum + nowGS.transform.forward;
            }
            sum = sum / overlapCount;
            Debug.DrawLine(transform.position, transform.position + sum * findingGravitySensorR, Color.green);
            return sum.normalized;
        }
        else
        {
            //找出最近的GS
            Collider nearestGS = null;
            float nearestDistance = float.MaxValue;
            Vector3 nowPos = transform.position;
            for (int i = 0; i < overlapCount; i++)
            {
                Collider nowGS = gs[i];

                float nowDistance = (nowGS.transform.position - nowPos).sqrMagnitude;
                if (nowDistance < nearestDistance)
                {
                    nearestGS = nowGS;
                    nearestDistance = nowDistance;
                }
            }
            Debug.DrawLine(nearestGS.transform.position, nearestGS.transform.position + nearestGS.transform.forward * findingGravitySensorR);
            return nearestGS.transform.forward;

        }
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
        groundUp = findGroundUp();  

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
        if (Physics.Raycast(from, -groundUp, out hit, 5, layerMask))
        {
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
            Vector3 controllForce = moveController.getMoveForce();

            //把controllForce投影到地面(期望可以貼著地面移動)
            controllForce = Vector3.ProjectOnPlane(controllForce, groundUp);
            controllForce.Normalize();

            //更新面向begin
            Vector3 forward2 = controllForce;
            if (forward2 != Vector3.zero)
            {
                Quaternion targetRotation2 = Quaternion.LookRotation(forward2, groundUp);
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

        //print("rigid="+rigid.velocity.magnitude);

        if(rigid.velocity.magnitude>0.01f)
        Debug.DrawLine(transform.position, transform.position + rigid.velocity*10/ rigid.velocity.magnitude, Color.blue);
    }
 
}
