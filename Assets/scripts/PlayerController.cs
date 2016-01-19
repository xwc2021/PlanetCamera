using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public interface FollowCameraBehavior
{
    void rotateByAxis(float angle, Vector3 axis);
}


public class PlayerController : MonoBehaviour {

    public FollowCameraBehavior cameraBehavior;
    public MonoBehaviour cameraSocket;
    public Transform laddingPlanet;
    public Rigidbody rigid;
    public float rotationSpeed = 0.6f;
    public float gravityScale = 1.4f;
    public float centripetalScale = 0.6f;
    public float moveSpeed = 1;
    Transform m_Cam;

    // Use this for initialization
    void Start () {
        m_Cam = Camera.main.transform;
        previouPosistion = transform.position;

        if (cameraSocket != null)
            cameraBehavior = cameraSocket as FollowCameraBehavior;

        //print("cameraBehavior="+cameraBehavior);
    }

    Vector3 previouPosistion;

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 planetGravity =laddingPlanet.position - transform.position;
        rigid.AddForce(gravityScale * planetGravity);

        Vector3 headUp = -planetGravity.normalized;
        Vector3 forward = Vector3.Cross(transform.right, headUp);
        Quaternion targetRotation =Quaternion.LookRotation(forward, headUp);
        transform.rotation = targetRotation;

        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {

            Vector3 nowVelocity = h * m_Cam.right + v * m_Cam.up;
            nowVelocity = Vector3.ProjectOnPlane(nowVelocity, headUp);
            nowVelocity.Normalize();

            //更新方向begin
            Vector3 forward2 = nowVelocity;
            if (forward2 != Vector3.zero)
            {
                Quaternion targetRotation2 = Quaternion.LookRotation(forward2, headUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
            }
            //end

            //加上向心速度
            Vector3 predictionPos = transform.position + Time.deltaTime * nowVelocity;
            Vector3 centripetalVelocity = laddingPlanet.position - predictionPos;
            centripetalVelocity.Normalize();

            nowVelocity = moveSpeed * (nowVelocity + centripetalScale * centripetalVelocity);

            Vector3 verticalV =Vector3.Project(rigid.velocity, headUp);//保留地心引力
            rigid.velocity = verticalV+nowVelocity;
            
            Debug.DrawLine(transform.position, transform.position + rigid.velocity, Color.blue);
        }

        //如果位置有更新，就更新FlowPoint
        //透過headUp和向量(nowPosition-previouPosistion)的外積，找出旋轉軸Z
        //用A軸來旋轉CameraPivot

        Vector3 diffV = transform.position - previouPosistion;
        Vector3 Z = Vector3.Cross(headUp, diffV);

        Vector3 from = (previouPosistion - laddingPlanet.position).normalized;
        Vector3 to = (transform.position - laddingPlanet.position).normalized;
        float cosValue =Vector3.Dot(from,to);
        float rotDegree =Mathf.Acos(cosValue) * Mathf.Rad2Deg;

        if(cameraBehavior != null)
            cameraBehavior.rotateByAxis(rotDegree, Z);

         previouPosistion = transform.position;
    }
}
