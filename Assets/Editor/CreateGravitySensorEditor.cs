

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(CreateGravitySensor))]
public class CreateGravitySensorEditor : UnityEditor.Editor
{
    CreateGravitySensor behavior;
    void OnEnable()
    {
        behavior = (CreateGravitySensor)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("createGS"))
        {
            behavior.createGS();
        }
    
    }
}
