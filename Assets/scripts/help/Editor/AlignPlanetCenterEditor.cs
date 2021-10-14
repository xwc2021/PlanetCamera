using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(AlignPlanetCenter))]
public class AlignPlanetCenterEditor : UnityEditor.Editor
{

    AlignPlanetCenter behavior;
    void OnEnable()
    {
        behavior = (AlignPlanetCenter)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("AlignPlanetCenter"))
        {
            behavior.align();
        }

    }
}
