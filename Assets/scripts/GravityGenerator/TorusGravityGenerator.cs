using UnityEngine;
public class TorusGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public float R = 10.0f;
    public float r = 2.0f;
    public Vector3 findGroundUp(Vector3 headUp, ref Vector3 targetPos)
    {
        var v = transform.InverseTransformPoint(targetPos);
        var radian = Mathf.Atan2(v.y, v.x);

        var axisX = transform.right;
        var axisY = transform.up;
        var gravicty_p = transform.position + R * (axisX * Mathf.Cos(radian) + axisY * Mathf.Sin(radian));

        var temp = Vector3.Normalize(targetPos - gravicty_p);
        return temp;
    }
}