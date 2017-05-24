using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PeekGeo))]
public class PeekGeoEditor : UnityEditor.Editor
{

    PeekGeo behavior;
    void OnEnable()
    {
        behavior = (PeekGeo)target;
    }

    string index="0";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        
        index = GUILayout.TextField(index);

        int i = int.Parse(index);
        if (GUILayout.Button("createTri3Axis"))
        {
            behavior.createTri3Axis(i);
        }

        if (GUILayout.Button("clear all"))
        {
            behavior.clearAll();
        }

    }
}
