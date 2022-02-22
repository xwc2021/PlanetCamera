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

    // 這些點z都是0
    public static void CalculateBarycentricCoordinates(ref Vector3 s0, ref Vector3 s1, ref Vector3 s2, ref Vector3 P, out bool isGetValue, out float a, out float b, out float r)
    {
        var diff = P - s0;

        // https://1.bp.blogspot.com/-csct6vAS1vs/YajSLKgW1WI/AAAAAAABDd8/shSLs78p274fnRa3R5fv3wN9dNS5WCN3wCPcBGAsYHg/s4618/screen_sapce.png
        // 求ray(P,S0-S2)和ray(S0,S1-S2)的交點
        // 等同於求ray(P,S0-S2)和平面的交點
        var dir01 = s1 - s0;
        var dir02 = s2 - s0;

        var help = Vector3.Cross(dir01, dir02);
        var n = Vector3.Cross(help, dir01); // 不需要正規化
        Debug.DrawRay((s0 + s1) / 2, n.normalized, Color.red);
        Debug.DrawRay(P, -dir02.normalized, Color.yellow);
        Vector3 hitPos;
        var result = RayHitPlane(P, -dir02, n, s0, out hitPos);
        if (!result)
        {
            Debug.DrawRay(hitPos, help * 10, Color.green);
            Debug.Log("RayHitPlane失敗");
            isGetValue = false;
        }

        var p_on_dir01 = hitPos;
        Debug.DrawRay(p_on_dir01, Vector3.up, Color.green);
        var vector_alpha = p_on_dir01 - s0;
        var vector_Beta = diff - vector_alpha;

        // 擋掉dir01、dir02是y軸平行的情況
        a = floatEqual(dir01.x, 0) ? vector_alpha.y / dir01.y : vector_alpha.x / dir01.x;
        b = floatEqual(dir02.x, 0) ? vector_Beta.y / dir02.y : vector_Beta.x / dir02.x;

        r = 1 - a - b;
        isGetValue = true;
    }

    public static bool floatEqual(float a, float b)
    {
        return Mathf.Abs(a - b) < float.Epsilon;
    }

    public static Vector3 CalculateInterpolationValueByBarycentricCoordinates(ref Vector3 s0, ref Vector3 s1, ref Vector3 s2, float a, float b, float r)
    {
        return s0 * r + s1 * a + s2 * b;
    }

    public static bool isInTriangle(float a, float b, float r)
    {
        return (a >= 0 && b >= 0 && r >= 0);
    }

    public static Vector3 CalculateTriangleNormal(ref Vector3 s0, ref Vector3 s1, ref Vector3 s2)
    {
        var dir01 = s1 - s0;
        var dir02 = s2 - s0;
        return Vector3.Cross(dir01, dir02).normalized;
    }
}