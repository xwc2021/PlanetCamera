using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetMovable))]
public class PlanetMovableInspector : Editor {

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("this component need a MoveController");
        DrawDefaultInspector();
    }
}
