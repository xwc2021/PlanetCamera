using UnityEngine;
using System.Collections;

public class MirrorCamera : MonoBehaviour {

    public bool useObliqueMatrix = true;
    public Camera refCamera;
    public Transform targetMirror;
    public Material mirrorMaterial;
    public Transform testPoint;

    Matrix4x4 P;
    Camera myCamera;
    // Use this for initialization
    void Start () {
        myCamera = GetComponent<Camera>();
        P = myCamera.projectionMatrix;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (refCamera == null)
            return;

        Vector3 normal = targetMirror.up;
        Vector3 mirrorZ = Vector3.Reflect(refCamera.transform.forward, normal);
        Vector3 mirrorY = Vector3.Reflect(refCamera.transform.up, normal);
        transform.rotation = Quaternion.LookRotation(mirrorZ, mirrorY);

        //計算position
        Vector3 temp = targetMirror.position-refCamera.transform.position ;
        transform.position = targetMirror.position - Vector3.Reflect(temp, normal);

        //Debug.DrawLine(transform.position, transform.position+transform.forward*10, Color.green);
        //Debug.DrawLine(refCamera.transform.position, refCamera.transform.position+ refCamera.transform.forward * 10, Color.blue);

        //從unity 內建的water4來的(早知道unity有內建的水，我就不會自己作了吧)
        //為了clip掉水下的vertex不畫
        if (useObliqueMatrix)
        {
            Vector4 clipPlane = CameraSpacePlane(myCamera, targetMirror.position, normal, 1.0f);
            myCamera.projectionMatrix = CalculateObliqueMatrix(P, clipPlane);
        }
        else
            myCamera.projectionMatrix = P;

        //set data to shader
        Matrix4x4 V = myCamera.worldToCameraMatrix;
        mirrorMaterial.SetMatrix("_mirror_camera_vp", P * V);
    }

    static float Sgn(float a)
    {
        if (a > 0.0F)
        {
            return 1.0F;
        }
        if (a < 0.0F)
        {
            return -1.0F;
        }
        return 0.0F;
    }

    public float clipPlaneOffset = 0.07F;

    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * clipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;

        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            Sgn(clipPlane.x),
            Sgn(clipPlane.y),
            1.0F,
            1.0F
            );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];

        return projection;
    }
}
