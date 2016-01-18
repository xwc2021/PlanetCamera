using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CameraPivot : MonoBehaviour {

    public bool flow = true;
    public float flowSpeed = 5;
    public float pitchSpeed = 2;
    public float yawSpeed = 5;
    public float Rspeed = 10;

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

        if (flow)
            recordPos = Vector3.Lerp(recordPos, myParent.position, flowSpeed * Time.deltaTime);
        else
            recordPos = myParent.position;

        transform.position = recordPos;

        float deltaY = CrossPlatformInputManager.GetAxis("Mouse Y");
        Quaternion pitch = Quaternion.Euler(pitchSpeed * deltaY, 0, 0);
        rot =  rot* pitch;

        float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");
        Quaternion yaw = Quaternion.AngleAxis(yawSpeed * deltaX,myParent.up);
        rot = yaw*rot ;

        transform.rotation = rot;

        float Rscale = Input.GetAxis("Mouse ScrollWheel");
        R += Rspeed * Rscale;
        R = Mathf.Max(2, R);

        camera.localPosition = new Vector3(0,0,-R);
        Debug.DrawLine(transform.position, camera.position, Color.red);
    }

    public void rotateByAxis(float angle, Vector3 axis)
    {
        if (angle < Mathf.Epsilon || float.IsNaN(angle))
            return;

        //print(angle);
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rot = q * rot;
    }
}
