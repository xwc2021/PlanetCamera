using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CameraPivot : MonoBehaviour, FollowCameraBehavior
{
    public float rotateByAxisScale = 0.5f;
    public bool follow = true;
    public float followSpeed = 5;
    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;
    public float Rdiff = 300;

    Vector3 recordPos;
    Quaternion rot;
    Transform myParent;
    Transform camera;
    float R;
    // Use this for initialization
    void Start () {
        rot = transform.rotation;
        myParent = transform.parent;
        camera = transform.GetChild(0);
        recordPos = transform.position;
        R = (transform.position - camera.position).magnitude;
    }
	
	// Update is called once per frame
	void Update () {

        if (follow)
            recordPos = Vector3.Lerp(recordPos, myParent.position, followSpeed * Time.deltaTime);
        else
            recordPos = myParent.position;

        transform.position = recordPos;

        float deltaY = CrossPlatformInputManager.GetAxis("Mouse Y");
        Quaternion pitch = Quaternion.Euler(perPitchDegreen * deltaY * Time.deltaTime, 0, 0);
        rot =  rot* pitch;

        float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");
        Quaternion yaw = Quaternion.AngleAxis(perYawDegreen * deltaX * Time.deltaTime, myParent.up);
        rot = yaw*rot ;

        transform.rotation = rot;

        float Rscale = Input.GetAxis("Mouse ScrollWheel");
        R += Rdiff * Rscale * Time.deltaTime;
        R = Mathf.Max(2, R);

        camera.localPosition = new Vector3(0,0,-R);
        Debug.DrawLine(transform.position, camera.position, Color.red);
    }

    public void rotateByAxis(float angle, Vector3 axis)
    {
        if (angle < Mathf.Epsilon || float.IsNaN(angle))
            return;

        //print(angle);
        Quaternion q = Quaternion.AngleAxis(angle* rotateByAxisScale, axis);
        rot = q * rot;
    }
}
