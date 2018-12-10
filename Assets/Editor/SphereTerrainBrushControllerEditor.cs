using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SphereTerrainBrushController))]
public class SphereTerrainBrushControllerEditor : UnityEditor.Editor
{
    SphereTerrainBrushController controller;
    void OnEnable()
    {
        controller = (SphereTerrainBrushController)target;
    }

    bool isUsingBrush = false;
    public void OnSceneGUI()
    {
        if (Event.current.button == 1)//right button
        {
            Vector3 from, dir;
            GeometryTool.GetShootingRay(Event.current.mousePosition, out from, out dir);
            Vector3 hitPoint, hitNormal;
            controller.from.position = from;
            var isHitSphere = rayHitSphere(from, dir, out hitPoint, out hitNormal);
            if (!isHitSphere)
                return;


            Vector3 hitOnPlane;
            var hitTerrain = getHitPlane(hitPoint, hitNormal, out hitOnPlane);
            if (hitTerrain == null)
            {
                Debug.Log("no hit plane");
                return;
            }

            if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
            {
                Debug.Log(hitTerrain.name);
                isUsingBrush = true;

                hitTerrain.setBrushLocalPosFrom(hitOnPlane);
                hitTerrain.useBrush(true);

                hitTerrain.updateNeighborsBrush(true);
                Event.current.Use(); // 中斷鏡頭的旋轉
                return;
            }

            if (isUsingBrush == true)
            {
                isUsingBrush = false;

                hitTerrain.setBrushLocalPosFrom(hitOnPlane);
                hitTerrain.useBrush(false);

                hitTerrain.updateNeighborsBrush(false);
            }
        }
    }

    bool rayHitSphere(Vector3 from, Vector3 dir, out Vector3 hitPoint, out Vector3 hitNormal)
    {
        var shereWorldCenter = controller.getSphereWorldCenter(); // 先射球
        var R = SphereTerrain.R;
        var isHit = GeometryTool.RayMarchingSphere(from, dir, shereWorldCenter, R, out hitPoint, out hitNormal);
        if (isHit)
        {
            var tangent = Vector3.zero;
            Vector3.OrthoNormalize(ref hitNormal, ref tangent);
            controller.to.rotation = Quaternion.LookRotation(hitNormal, tangent);
            controller.to.position = hitPoint;
        }
        else
        {
            controller.to.position = from + dir * 1000.0f;
            controller.to.rotation = Quaternion.identity;
        }
        return isHit;
    }

    bool rayHitPlane(Vector3 from, Vector3 dir, SphereTerrain sTerrain, out Vector3 hitPoint)
    {
        if (!GeometryTool.RayHitPlane(from, dir, sTerrain.getPlaneNormal(), sTerrain.getPlanePoint(), out hitPoint))
            return false;

        var localHitPoint = sTerrain.transform.InverseTransformPoint(hitPoint);
        var half = 1023.0f * 0.5f;
        var inRect = Mathf.Abs(localHitPoint.x) < half && Mathf.Abs(localHitPoint.z) < half;
        controller.hitOnPlane.position = inRect ? hitPoint : Vector3.zero;
        return inRect;
    }

    SphereTerrain getHitPlane(Vector3 from, Vector3 dir, out Vector3 hitPoint)
    {
        var sphereTerrains = controller.sphereTerrains;
        for (var i = 0; i < sphereTerrains.Length; ++i)
        {
            var sTerrain = sphereTerrains[i];
            if (rayHitPlane(from, dir, sTerrain, out hitPoint))
                return sTerrain;
        }

        hitPoint = Vector3.zero;
        return null;
    }
}
