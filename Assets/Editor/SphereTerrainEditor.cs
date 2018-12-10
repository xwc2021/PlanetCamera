using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SphereTerrain))]
public class SphereTerrainEditor : UnityEditor.Editor
{
    SphereTerrain behavior;
    void OnEnable()
    {
        behavior = (SphereTerrain)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("create64Piece"))
        {
            behavior.create64Piece();
        }
    }

    bool isUsingBrush = false;
    public void OnSceneGUI()
    {
        var hitPointWorld = ShootRay(Event.current.mousePosition);
        if (Event.current.button == 1)//right button
        {
            // var hitPointWorld = ShootRay(Event.current.mousePosition);
            if (Event.current.type == EventType.MouseDrag)
            {
                behavior.setBrushLocalPos(hitPointWorld);
                behavior.useBrush(isUsingBrush = true);
                Event.current.Use(); // 中斷鏡頭的旋轉
                return;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                behavior.setBrushLocalPos(hitPointWorld);
                behavior.useBrush(isUsingBrush = true);
                return;
            }

            if (isUsingBrush == true)
            {
                behavior.useBrush(isUsingBrush = false);
            }
        }
    }

    Vector3 ShootRay(Vector3 mousePos)
    {
        Vector3 from, dir;
        GeometryTool.GetShootingRay(mousePos, out from, out dir);
        Vector3 hitPoint, hitNormal;
        // GeometryTool.RayHitPlane(from, dir, behavior.getPlaneNormal(), behavior.getPlanePoint(), out hitPoint);
        var shereWorldCenter = behavior.getSphereWorldCenter(); // 先射球
        var isHit = GeometryTool.RayMarchingSphere(from, dir, behavior.getSphereWorldCenter(), behavior.getSphereR(), out hitPoint, out hitNormal);

        behavior.from.position = from;
        Vector3 hitOnPlane = Vector3.zero;
        if (isHit)
        {
            var tangent = Vector3.zero;
            Vector3.OrthoNormalize(ref hitNormal, ref tangent);
            behavior.to.rotation = Quaternion.LookRotation(hitNormal, tangent);

            // 再射向平面
            GeometryTool.RayHitPlane(hitPoint, hitNormal, behavior.getPlaneNormal(), behavior.getPlanePoint(), out hitOnPlane);

            behavior.to.position = hitPoint;
            behavior.hitOnPlane.position = hitOnPlane;
            // behavior.to.rotation = Quaternion.identity;
        }
        else
        {
            behavior.to.position = from + dir * 1000.0f;
            behavior.to.rotation = Quaternion.identity;
        }

        return hitOnPlane;
    }
}
