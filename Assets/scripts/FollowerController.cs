using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FollowerController : MonoBehaviour {

    public Transform laddingPlanet;
    public Rigidbody rigid;
    public float rotationSpeed = 0.6f;
    public float gravityScale = 1.4f;
    public float centripetalScale = 0.6f;
    public float moveSpeed = 1;
    public Transform followTarget;

    // Use this for initialization
    void Start () {

    }

    


    Vector2 memoryPos;
    Vector2 nowInput;
    void doAndroidInput()
    {
        if (Input.touchCount == 1)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                memoryPos = Input.touches[0].position;
            }

            if (Input.touches[0].phase == TouchPhase.Moved && Input.touches[0].phase != TouchPhase.Canceled)
            {

                nowInput = Input.touches[0].position - memoryPos;
                //nowInput = Input.touches[0].deltaPosition;
            }

            else if (Input.touches[0].phase == TouchPhase.Ended)
                nowInput = new Vector2(0, 0);
        }

    }




    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 planetGravity = laddingPlanet.position - transform.position;
        rigid.AddForce(gravityScale * planetGravity);

        Vector3 headUp = -planetGravity.normalized;
        Vector3 forward = Vector3.Cross(transform.right, headUp);
        Quaternion targetRotation = Quaternion.LookRotation(forward, headUp);
        transform.rotation = targetRotation;


        {
            Vector3 diff = followTarget.position - transform.position; ;
            if (diff.magnitude < 0.5)
                return;

            Vector3 nowVelocity = diff;
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

            Vector3 verticalV = Vector3.Project(rigid.velocity, headUp);//保留地心引力
            rigid.velocity = verticalV + nowVelocity;

            Debug.DrawLine(transform.position, transform.position + rigid.velocity, Color.blue);


        }
    }

}
