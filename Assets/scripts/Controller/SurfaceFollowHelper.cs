using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class SurfaceFollowHelper : MonoBehaviour
{
    public CameraPivot cameraPivot;
    PlanetMovable planetMovable;
    Vector3 previousGroundUp;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        previousGroundUp = transform.up;
    }

    public void doAdjustByGroundUp()
    {
        // 找出旋轉軸Z
        Vector3 groundUp = planetMovable.UpDir;
        Vector3 Z = Vector3.Cross(previousGroundUp, groundUp);
        // Debug.DrawLine(transform.position, transform.position + Z * 16, Color.blue);
        // Debug.DrawLine(transform.position, transform.position + previousGroundUp * 16, Color.red);
        // Debug.DrawLine(transform.position, transform.position + groundUp * 16, Color.green);

        // 找出旋轉角度
        // http://answers.unity3d.com/questions/778626/mathfacos-1-return-nan.html
        float cosValue = Vector3.Dot(previousGroundUp, groundUp);
        cosValue = Mathf.Clamp(cosValue, -1.0f, 1.0f);
        float rotDegree = Mathf.Acos(cosValue) * Mathf.Rad2Deg;
        // print("rotDegree=" + rotDegree);

        // 超過threshold才更新
        float threshold = 0.1f;
        if (rotDegree > threshold)
        {
            //print("rotDegree=" + rotDegree);
            Quaternion q = Quaternion.AngleAxis(rotDegree, Z);
            cameraPivot.setSurfaceAdjust(true, q);

            previousGroundUp = groundUp;
        }
    }
}