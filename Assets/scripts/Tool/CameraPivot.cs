using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CameraPivot : MonoBehaviour, FollowCameraBehavior
{
    public float rotateFollowSpeed = 5;
    public bool follow = true;
    public float posFollowSpeed = 5;
    public float perPitchDegreen = 200;
    public float perYawDegreen = 600;
    public float Rdiff = 300;

    Vector3 recordPos;
    Quaternion rot;
    Transform myParent;
    Transform CAMERA;
    float R;
    // Use this for initialization
    void Start () {
        rot = transform.rotation;
        myParent = transform.parent;
        CAMERA = transform.GetChild(0);
        recordPos = transform.position;
        R = (transform.position - CAMERA.position).magnitude;
    }
	
	void LateUpdate() {

        if (follow)
            recordPos = Vector3.Lerp(recordPos, myParent.position, posFollowSpeed * Time.deltaTime);
        else
            recordPos = myParent.position;

        transform.position = recordPos;

        //從此之後，rot永遠在local space運作(它的parent space是temporary)
        float deltaY = CrossPlatformInputManager.GetAxis("Mouse Y");
        Quaternion pitch = Quaternion.Euler(perPitchDegreen * deltaY * Time.deltaTime, 0, 0);
        rot = rot * pitch;

        float deltaX = CrossPlatformInputManager.GetAxis("Mouse X");
        //Quaternion yaw = Quaternion.AngleAxis(perYawDegreen * deltaX * Time.deltaTime, myParent.up);
        Quaternion yaw = Quaternion.Euler(0, perYawDegreen * deltaX * Time.deltaTime, 0);

        rot = yaw * rot;

        if (!doRotateFollow)
            transform.rotation = rot;
        else
        {
            if (isFirst)
            {
                temporary = Quaternion.identity;
                isFirst = false;
            } 

            temporary = Quaternion.Slerp(temporary, sumAdjustRot, rotateFollowSpeed * Time.deltaTime);
            transform.rotation = temporary* rot;
        }

        float Rscale = Input.GetAxis("Mouse ScrollWheel");
        R += Rdiff * Rscale * Time.deltaTime;
        R = Mathf.Max(2, R);

        CAMERA.localPosition = new Vector3(0,0,-R);
        Debug.DrawLine(transform.position, CAMERA.position, Color.red);
    }

    Quaternion temporary;
    Quaternion sumAdjustRot=Quaternion.identity;
    bool doRotateFollow = false;
    bool isFirst = true;

    public void setAdjustRotate(bool doRotateFollow, Quaternion adjustRotate)
    {
        sumAdjustRot = adjustRotate*sumAdjustRot;
        this.doRotateFollow = doRotateFollow;
    }
}
