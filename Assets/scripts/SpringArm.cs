using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


//舊方法，已改成使用CameraPivot
public class SpringArm : MonoBehaviour, FollowCameraBehavior
{

    public float maxPitchDegree = 60;
    public float minPitchDegree = -80;

    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;
    public float Rdiff = 300;

    public bool follow = true;
    public float followSpeed = 5;

    Transform myParent;

    Vector3 dirInWorld;
    float R;
    Vector3 recordPos;
    Vector3 recordRight;
	// Use this for initialization
	void Start () {

        myParent = transform.parent;
        recordPos = myParent.position;
        dirInWorld = transform.position - myParent.position;
        R = dirInWorld.magnitude;
        dirInWorld.Normalize();
        transform.rotation = Quaternion.LookRotation(-dirInWorld, myParent.up);
        recordRight = transform.right;
    }

    public void rotateByAxis(float angle, Vector3 axis)
    {
        if (angle < Mathf.Epsilon || float.IsNaN(angle))
            return;

        //print(angle);
        Quaternion q =Quaternion.AngleAxis(angle, axis);
        Matrix4x4 rot =Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        dirInWorld = rot * dirInWorld;
    }

    // Update is called once per frame
    void Update () {

        if (follow)
            recordPos = Vector3.Lerp(recordPos, myParent.position, followSpeed * Time.deltaTime);
        else
            recordPos = myParent.position;

        float nowPitchDegree = 0;
        //handle pitch
        {
            float deltaY = CrossPlatformInputManager.GetAxis("Mouse Y");

            //Vector3 pitchHelpX = Vector3.Cross(dirInWorld, myParent.up);
            //Vector3 pitchHelpZ = Vector3.Cross(pitchHelpX, myParent.up);
            //原來的作法，在nowPitchDegree>0時，pitchHelpZ會差180度
            Vector3 pitchHelpZ = Vector3.Cross(recordRight, myParent.up);
            pitchHelpZ.Normalize();

            Vector2 c = new Vector2(Vector3.Dot(dirInWorld, myParent.up), Vector3.Dot(dirInWorld, pitchHelpZ));

            nowPitchDegree = Mathf.Atan2(c.y, c.x)*Mathf.Rad2Deg;

            //在helpX軸上轉動
            //Complex multiplication
            //(x+yi)*(cos(theda)+sin(theda)i) = x*cos(theda)-y*sin(theda)+i(y*cos(theda)+x*sin(theda)) 
 
            print("newPitchDegree="+nowPitchDegree);
            if (minPitchDegree <= nowPitchDegree && nowPitchDegree <= maxPitchDegree)
            {
                float addPitchDegree = perPitchDegreen * deltaY * Time.deltaTime;
                float newPitchDegree = nowPitchDegree + addPitchDegree;
                if (minPitchDegree <= newPitchDegree && newPitchDegree <= maxPitchDegree)
                {
                    float addPitchRad = addPitchDegree * Mathf.Deg2Rad;

                    float real = c.x * Mathf.Cos(addPitchRad) - c.y * Mathf.Sin(addPitchRad);
                    float imaginary = c.y * Mathf.Cos(addPitchRad) + c.x * Mathf.Sin(addPitchRad);

                    dirInWorld = real * myParent.up + imaginary * pitchHelpZ;

                    //更新nowPitchDegree，因為之後會用到
                    nowPitchDegree = nowPitchDegree + addPitchDegree;
                } 
            }
        }

        //handle yaw
        {
            Vector3 projection = Vector3.Project(dirInWorld, myParent.up);
            Vector3 yawHelpX = dirInWorld - projection;
            float smallR = yawHelpX.magnitude;
            yawHelpX.Normalize();
            Vector3 yawHelpZ = Vector3.Cross(yawHelpX, myParent.up);

            float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");
            float yawDegree = perYawDegreen * deltaX * Time.deltaTime;

            float yawRad = yawDegree * Mathf.Deg2Rad;
            dirInWorld = smallR * Mathf.Cos(yawRad) * yawHelpX + smallR * Mathf.Sin(yawRad) * yawHelpZ + projection;
        }

        //handle R
        {
            float Rscale = Input.GetAxis("Mouse ScrollWheel");
            R += Rdiff * Rscale * Time.deltaTime;
            R = Mathf.Max(2, R);
        }

        dirInWorld.Normalize();
        transform.position = recordPos+ R* dirInWorld;
        Debug.DrawLine(transform.position, myParent.position, Color.red);

        //更新旋轉
        {
            Vector3 newCameraRight = Vector3.Cross(dirInWorld, myParent.up);
            if (nowPitchDegree > 0)
                newCameraRight = -newCameraRight;

            Vector3 newCameraUp = Vector3.Cross(newCameraRight, dirInWorld);
            transform.rotation = Quaternion.LookRotation(-dirInWorld, newCameraUp);
            recordRight = transform.right;
        }  
    }
}
