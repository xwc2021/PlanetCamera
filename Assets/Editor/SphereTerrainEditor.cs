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


        if (Event.current.button == 1)//right button
        {
            if (Event.current.type == EventType.MouseDrag)
            {
                var hitPointWorld = ShootRay(Event.current.mousePosition);
                behavior.setBrushLocalPos(hitPointWorld);
                behavior.useBrush(isUsingBrush = true);
                Debug.Log("isUsingBrush = " + isUsingBrush);
                return;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                var hitPointWorld = ShootRay(Event.current.mousePosition);
                behavior.setBrushLocalPos(hitPointWorld);
                behavior.useBrush(isUsingBrush = true);
                Debug.Log("isUsingBrush = " + isUsingBrush);
                return;
            }

            if (isUsingBrush == true)
            {
                behavior.useBrush(isUsingBrush = false);
                Debug.Log("isUsingBrush = " + isUsingBrush);
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
        return hitPoint;
    }
}
