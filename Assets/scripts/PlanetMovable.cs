using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;



public interface InputProxy
{
    Vector2 getHV();
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


        if(moveController!=null)
        {
            Vector3 controllForce = moveController.getMoveForce();

            //把controllForce投影到地面(期望可以貼著地面移動)
            controllForce = Vector3.ProjectOnPlane(controllForce, headUp);
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
            rigid.AddForce(moveForceScale * controllForce);
        }

        //加上重力
        rigid.AddForce(gravityScale * planetGravity);

        Debug.DrawLine(transform.position, transform.position + rigid.velocity, Color.blue);
    }
 
}
