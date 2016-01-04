using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FlowPoint : MonoBehaviour {

    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;

    Transform target;

    Vector3 dirInWorld;
    float R;
    float smallR;
	// Use this for initialization
	void Start () {

        recordFirstTime();
    }

    void recordFirstTime()
    {
        target = transform.parent;
        dirInWorld = transform.position - target.position;
        R = dirInWorld.magnitude;
        dirInWorld.Normalize();
        transform.rotation = Quaternion.LookRotation(-dirInWorld, target.up);
    }

    void resetData(Vector3 pos)
    {
        dirInWorld = pos - target.position;
        dirInWorld.Normalize();
    }

    float maxPitchDegree = 85;
    float minPitchDegree = 5;
    float epsilonDegree = 2;

    public Vector3 projection;

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

        Vector3 newPos = target.position + dirInWorld * R;

        //handle pitch
        Vector3 pitchHelpX = Vector3.Cross(dirInWorld, target.up);
        Vector3 pitchHelpZ = Vector3.Cross(pitchHelpX, dirInWorld);
        //在helpX軸上轉動

        Vector2 upLocal = new Vector2(Vector3.Dot(target.up, dirInWorld), Vector3.Dot(target.up, pitchHelpZ));
        //相對於camera的角度
        float nowPitchDegree = Mathf.Atan2(upLocal.y, upLocal.x) * Mathf.Rad2Deg;
        //print("nowPitchDegree" + nowPitchDegree);

        float deltaY = CrossPlatformInputManager.GetAxis("Mouse Y");
        if (nowPitchDegree >= minPitchDegree && nowPitchDegree <= maxPitchDegree)
        {
            
            float pitchDegree = perPitchDegreen * deltaY * Time.deltaTime;
            if (pitchDegree > 0)
                pitchDegree = Mathf.Min(pitchDegree, nowPitchDegree - minPitchDegree - epsilonDegree);
            else if (pitchDegree < 0)
                pitchDegree = -Mathf.Min(-pitchDegree, maxPitchDegree - nowPitchDegree - epsilonDegree);

            float pitchRad = pitchDegree * Mathf.Deg2Rad;
            newPos = R * Mathf.Cos(pitchRad) * dirInWorld + R * Mathf.Sin(pitchRad) * pitchHelpZ + target.position;
            resetData(newPos);
        }

        //handle yaw
        Vector3 v = newPos - target.position;
        projection = Vector3.Project(v, target.up);
        Vector3 yawHelpX = v - projection;
        smallR = yawHelpX.magnitude;
        yawHelpX.Normalize();
        Vector3 yawHelpZ = Vector3.Cross(yawHelpX, target.up);

        float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");
        float yawDegree = perYawDegreen * deltaX * Time.deltaTime;

        float yawRad = yawDegree * Mathf.Deg2Rad;
        newPos = smallR * Mathf.Cos(yawRad) * yawHelpX + smallR * Mathf.Sin(yawRad) * yawHelpZ + target.position + projection;
        resetData(newPos);

        transform.position = newPos;
        Vector3 newCameraRight = Vector3.Cross(dirInWorld, target.up);
        Vector3 newCameraUp = Vector3.Cross(-dirInWorld, newCameraRight);
        transform.rotation = Quaternion.LookRotation(-dirInWorld, newCameraUp);

        Debug.DrawLine(transform.position, target.position, Color.red);
    }
}
