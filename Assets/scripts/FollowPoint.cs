using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


//舊方法，已改成使用CameraPivot
public class FollowPoint : MonoBehaviour {

    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;
    public float Rspeed = 10;

    public bool follow = true;
    public float followSpeed = 5;

    Transform myParent;

    public Vector3 dirInWorld;
    float R;
    
    Vector3 recordPos;
	// Use this for initialization
	void Start () {

        myParent = transform.parent;
        recordPos = myParent.position;
        dirInWorld = transform.position - myParent.position;
        R = dirInWorld.magnitude;
        dirInWorld.Normalize();
        transform.rotation = Quaternion.LookRotation(-dirInWorld, myParent.up);
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

        //handle pitch
        {
            float deltaY = CrossPlatformInputManager.GetAxis("Mouse Y");

            Vector3 pitchHelpX = Vector3.Cross(dirInWorld, myParent.up);
            Vector3 pitchHelpZ = Vector3.Cross(pitchHelpX, myParent.up);
            pitchHelpZ.Normalize();

            Vector2 c = new Vector2(Vector3.Dot(dirInWorld, myParent.up), Vector3.Dot(dirInWorld, pitchHelpZ));

            float nowPitchDegree =Mathf.Atan2(c.y, c.x)*Mathf.Rad2Deg;

            //在helpX軸上轉動
            //Complex multiplication
            //(x+yi)*(cos(theda)+sin(theda)i) = x*cos(theda)-y*sin(theda)+i(y*cos(theda)+x*sin(theda)) 

            const float maxDegree = -10;
            const float minDegree = -170;
            print("newPitchDegree="+nowPitchDegree);
            if (minDegree <= nowPitchDegree && nowPitchDegree <= maxDegree)
            {
                float addPitchDegree = perPitchDegreen * deltaY * Time.deltaTime;
                float newPitchDegree = nowPitchDegree + addPitchDegree;
                if (minDegree <= newPitchDegree && newPitchDegree <= maxDegree)
                {
                    float addPitchRad = addPitchDegree * Mathf.Deg2Rad;

                    float real = c.x * Mathf.Cos(addPitchRad) - c.y * Mathf.Sin(addPitchRad);
                    float imaginary = c.y * Mathf.Cos(addPitchRad) + c.x * Mathf.Sin(addPitchRad);

                    dirInWorld = real * myParent.up + imaginary * pitchHelpZ;
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
            R += Rspeed * Rscale;
            R = Mathf.Max(2, R);
        }

        transform.position = recordPos+ R* dirInWorld;
        Debug.DrawLine(transform.position, myParent.position, Color.red);

        Vector3 newCameraRight = Vector3.Cross(dirInWorld, myParent.up);
        Vector3 newCameraUp = Vector3.Cross(newCameraRight,dirInWorld);
        transform.rotation = Quaternion.LookRotation(-dirInWorld, newCameraUp);    
    }
}
