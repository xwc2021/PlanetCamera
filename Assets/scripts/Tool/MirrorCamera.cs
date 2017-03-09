using UnityEngine;
using System.Collections;

public class MirrorCamera : MonoBehaviour {

    public Camera refCamera;
    public Transform targetMirror;
    public Material mirrorMaterial;
    public Transform testPoint;

    Camera myCamera;
    // Use this for initialization
    void Start () {
        myCamera = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 normal = targetMirror.up;
        Vector3 mirrorZ = Vector3.Reflect(refCamera.transform.forward, normal);
        Vector3 mirrorY = Vector3.Reflect(refCamera.transform.up, normal);
        transform.rotation = Quaternion.LookRotation(mirrorZ, mirrorY);

        //計算position
        Vector3 temp = targetMirror.position-refCamera.transform.position ;
        transform.position = targetMirror.position - Vector3.Reflect(temp, normal);


        //Debug.DrawLine(transform.position, transform.position+transform.forward*10, Color.green);
        //Debug.DrawLine(refCamera.transform.position, refCamera.transform.position+ refCamera.transform.forward * 10, Color.blue);

        //set data to shader
        Matrix4x4 M =targetMirror.transform.localToWorldMatrix;
        Matrix4x4 V = myCamera.worldToCameraMatrix;
        Matrix4x4 P = myCamera.projectionMatrix;
        mirrorMaterial.SetMatrix("_mirror_camera_mvp", P * V * M);

        if (testPoint != null)
        {
            Vector4 v = testPoint.localPosition;
            v.w = 1;
            Vector4 tv = P * V * M * v;
            tv = tv / tv.w;
            //print("testPoint NDC = (" + tv.x + "," + tv.y + ")");
        }
    }
}
