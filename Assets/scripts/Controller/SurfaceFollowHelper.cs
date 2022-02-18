using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class SurfaceFollowHelper : MonoBehaviour
{
    public CameraPivot cameraPivot;
    PlanetMovable planetMovable;
    Vector3 previousUp;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        previousUp = transform.up;
    }

    public void doAdjustByGroundUp()
    {
        // 找出旋轉軸Z
        Vector3 up = planetMovable.UpDir;
        Vector3 Z = Vector3.Cross(previousUp, up);
        // Debug.DrawLine(transform.position, transform.position + Z * 16, Color.blue);
        // Debug.DrawLine(transform.position, transform.position + previousGroundUp * 16, Color.red);
        // Debug.DrawLine(transform.position, transform.position + groundUp * 16, Color.green);

        // 找出旋轉角度
        // http://answers.unity3d.com/questions/778626/mathfacos-1-return-nan.html
        float cosValue = Vector3.Dot(previousUp, up);
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

            previousUp = up;
        }
    }
}