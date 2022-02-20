using UnityEngine;
public class TorusGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public float R = 10.0f;
    public float r = 2.0f;
    public Vector3 findGravityDir(Vector3 headUp, Vector3 targetPos)
    {
        var v = transform.InverseTransformPoint(targetPos);
        var radian = Mathf.Atan2(v.y, v.x);

        var axisX = transform.right;
        var axisY = transform.up;
        var point_on_ring = transform.position + R * (axisX * Mathf.Cos(radian) + axisY * Mathf.Sin(radian));
        return Vector3.Normalize(point_on_ring - targetPos);
    }
}