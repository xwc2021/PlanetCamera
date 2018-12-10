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
        Vector3 hitPoint;
        GeometryTool.RayHitPlane(from, dir, behavior.getPlaneNormal(), behavior.getPlanePoint(), out hitPoint);

        behavior.from.position = from;
        behavior.to.position = hitPoint;
        behavior.to.rotation = behavior.transform.rotation;
        return hitPoint;
    }
}
