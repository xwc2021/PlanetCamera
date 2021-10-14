using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TorusGravityGeneratorGizmoDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoForMyScript(TorusGravityGenerator scr, GizmoType gizmoType)
    {
        Vector3 position = scr.transform.position;
        var R = scr.R;
        var r = scr.r;
        var axisX = scr.transform.right;
        var axisY = scr.transform.up;
        var axisZ = scr.transform.forward;

        //大圓
        drawCircle(position, R, axisX, axisY);

        //小圓

        drawCircle(position + axisX * R, r, axisX, axisZ);
    }

    static void drawCircle(Vector3 center, float R, Vector3 axisX, Vector3 axisY)
    {
        var n = 50;
        var two_pi = 2.0f * Mathf.PI;
        var diff = two_pi / n;
        for (var i = 0; i < n; ++i)
        {
            var radian = i * diff;
            var next_radian = (radian + diff) % two_pi;
            var from = center + R * (axisX * Mathf.Cos(radian) + axisY * Mathf.Sin(radian));
            var to = center + R * (axisX * Mathf.Cos(next_radian) + axisY * Mathf.Sin(next_radian));
            Debug.DrawLine(from, to, Color.green);
        }
    }
}