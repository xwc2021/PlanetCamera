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
    static void CalculateBarycentricCoordinates(Vector3 s0, Vector3 s1, Vector3 s2, Vector3 P, ref bool success, out float α, out float β, out float γ)
    {
        var diff = P - s0;

        // https://1.bp.blogspot.com/-csct6vAS1vs/YajSLKgW1WI/AAAAAAABDd8/shSLs78p274fnRa3R5fv3wN9dNS5WCN3wCPcBGAsYHg/s4618/screen_sapce.png
        // 求ray(P,S0-S2)和ray(S0,S1-S2)的交點
        // 等同於求ray(P,S0-S2)和平面的交點
        var dir01 = s1 - s0;
        var dir02 = s2 - s0;

        var help = Vector3.Cross(dir01, dir02);
        var n = Vector3.Cross(help, dir01); // 不需要正規化

        Vector3 hitPos;
        var result = RayHitPlane(P, -dir02, s0, n, out hitPos);
        if (!result)
        {
            // 退化成直線的三角形才有也可能
            // console.log('平行', s0, s1, s2, P);
            success = false;
        }

        var p_on_dir01 = hitPos;
        var vector_α = p_on_dir01 - s0;
        var vector_β = diff - vector_α;

        // 擋掉dir01、dir02是y軸平行的情況
        // 浮點數請用 number_equal，不然會GG
        // 見圖：bug/float_point_compaire_error(fixed)/bug_when_clipping_2.jpg
        // 其實當初直接用長度比算α、β不是更簡單嗎？
        α = vector_α.magnitude / dir01.magnitude;
        β = vector_β.magnitude / dir02.magnitude;

        γ = 1 - α - β;
    }
}