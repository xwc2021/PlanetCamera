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

        if (GUILayout.Button("updateLocalPos"))
        {
            behavior.updateLocalPos();
        }

        if (GUILayout.Button("create64Piece"))
        {
            behavior.create64Piece();
        }
    }

    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == 1)//right button
            {
                ShootRay(Event.current.mousePosition);
                SceneView.RepaintAll();
            }
        }
    }

    void ShootRay(Vector3 mousePos)
    {
        Vector3 from, dir;
        GeometryTool.GetShootingRay(mousePos, out from, out dir);

        behavior.from.position = from;
        behavior.to.position = from + dir * 500.0f;
    }
}
