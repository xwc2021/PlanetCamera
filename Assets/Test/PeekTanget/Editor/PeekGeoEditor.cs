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

   
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        

        if (GUILayout.Button("createTri3Axis"))
        {
            behavior.createTri3Axis();
        }

        if (GUILayout.Button("clear all"))
        {
            behavior.clearAll();
        }

    }
}
