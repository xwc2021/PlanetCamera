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
}
