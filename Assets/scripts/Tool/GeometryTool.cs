using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryTool
{
    // 之後再改成公式解
    static public bool RayMarchingSphere(Vector3 from, Vector3 dir, Vector3 sphereWorldCenter, float R, out Vector3 hitPos, out Vector3 hitNormal)
    {
        var oldDist = float.MaxValue;
        var p = from;
        for (var i = 0; i < 100; ++i)
        {
            var dist = (p - sphereWorldCenter).magnitude - R;
            if (dist > oldDist) // 比上次遠，代表射不中了
            {
                // Debug.Log(dist + ">" + oldDist + " " + i);
                break;
            }

            oldDist = dist;

            if (dist < 0.001f)
            {
                // Debug.Log("break" + i);
                hitPos = p;
                hitNormal = (hitPos - sphereWorldCenter).normalized;
                return true;
            }

            p += dir * dist;
        }

        hitNormal = hitPos = Vector3.zero;
        return false;
    }

    static public bool RayHitPlane(Vector3 from, Vector3 dir, Vector3 PlaneN, Vector3 PlaneC, out Vector3 hitPos)
    {
        //(F-C)。N + t (D。N) = 0
        // t  = (C-F)。N / (D。N)
        // t  = (A / (B)
        var B = Vector3.Dot(dir, PlaneN);
        var A = Vector3.Dot(PlaneC - from, PlaneN);

        var Epsilon = 0.0001f;
        if (Mathf.Abs(B) < Epsilon)
        {
            hitPos = Vector3.zero;
            return false;
        }
        var t = A / B;
        if (t < 0.0f)
        {
            hitPos = Vector3.zero;
            return false;
        }
        hitPos = from + t * dir;
        return true;
    }

    static void GetShootingRayPerspective(Vector3 mousePos, out Vector3 from, out Vector3 dir)
    {
        var camera = Camera.current;
        mousePos.y = camera.pixelHeight - mousePos.y;//mousePos左上角是(0,0)
        from = camera.transform.position;

        float clickPointDistance = 10.0f;
        var clickWorldPoint = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, clickPointDistance));

        dir = (clickWorldPoint - from).normalized;
    }

    //正交camera
    static void GetGetShootingRayOrthographic(Vector3 mousePos, out Vector3 from, out Vector3 dir)
    {
        var camera = Camera.current;
        mousePos.y = camera.pixelHeight - mousePos.y;//mousePos左上角是(0,0)

        var halfWidth = 0.5f * camera.pixelWidth;
        var halfHeight = 0.5f * camera.pixelHeight;
        var nx = (mousePos.x - halfWidth) / halfWidth;
        var ny = (mousePos.y - halfHeight) / halfHeight;
        var ratio = (float)camera.pixelWidth / camera.pixelHeight;

        var offsetY = ny * camera.orthographicSize;
        var offsetX = ratio * nx * camera.orthographicSize;
        from = camera.transform.position +
            offsetY * camera.transform.up +
            offsetX * camera.transform.right;

        dir = camera.transform.forward;
    }

    public static void GetShootingRay(Vector3 mousePos, out Vector3 from, out Vector3 dir)
    {
        var camera = Camera.current;
        if (camera.orthographic)
            GeometryTool.GetGetShootingRayOrthographic(mousePos, out from, out dir);
        else
            GeometryTool.GetShootingRayPerspective(mousePos, out from, out dir);
    }
}
