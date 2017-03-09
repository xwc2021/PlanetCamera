using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


public class PlayerControllerSimple : MonoBehaviour
{
    public Rigidbody rigid;
    public float rotationSpeed = 0.6f;
    public float moveSpeed = 1;
    Transform m_Cam;

    // Use this for initialization
    void Start()
    {
        m_Cam = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            RaycastHit hit;
            Vector3 hitNormal = transform.up;
            Vector3 from = Vector3.up + transform.position;

            //原本沒作mask會射到雪人自己，所以就飛起來了
            int layerMask = 1 << 10;
            if (Physics.Raycast(from, -Vector3.up, out hit,5,layerMask))
            {
                hitNormal = hit.normal;
            }

            Vector3 nowVelocity = h * m_Cam.right + v * m_Cam.up;
            nowVelocity = Vector3.ProjectOnPlane(nowVelocity, hitNormal);
            nowVelocity.Normalize();
            rigid.velocity = moveSpeed * nowVelocity;

            Vector3 forward = nowVelocity;
            forward.y = 0;
            if (forward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            Debug.DrawLine(transform.position, transform.position + rigid.velocity, Color.yellow);
        }
    }
}
