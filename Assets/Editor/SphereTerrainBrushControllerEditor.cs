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
        Vector3 from, dir;
        GeometryTool.GetShootingRay(Event.current.mousePosition, out from, out dir);
        Vector3 hitPoint, hitNormal;
        rayOnSphere(from, dir, out hitPoint, out hitNormal);

        if (Event.current.button == 1)//right button
        {
            SphereTerrain[] testingTerrainList;
            int testingTerrainCount;
            controller.getTestingTerrains(hitNormal, out testingTerrainList, out testingTerrainCount);

            // var hitPointWorld = ShootRay(Event.current.mousePosition);
            if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
            {
                usingBrush(hitPoint, hitNormal, testingTerrainList, testingTerrainCount);
                Event.current.Use(); // 中斷鏡頭的旋轉
                return;
            }

            if (isUsingBrush == true)
            {
                isUsingBrush = false;
                for (var i = 0; i < testingTerrainCount; ++i)
                {
                    var tesingTerrain = testingTerrainList[i];
                    tesingTerrain.useBrush(false);
                }
            }
        }
    }

    void usingBrush(Vector3 from, Vector3 dir, SphereTerrain[] testingTerrainList, int testingTerrainCount)
    {
        isUsingBrush = true;
        for (var i = 0; i < testingTerrainCount; ++i)
        {
            var tesingTerrain = testingTerrainList[i];

            var hitPointWorld = rayOnPlane(from, dir, tesingTerrain);
            tesingTerrain.setBrushLocalPos(hitPointWorld);
            tesingTerrain.useBrush(true);
        }
    }

    bool rayOnSphere(Vector3 from, Vector3 dir, out Vector3 hitPoint, out Vector3 hitNormal)
    {
        // GeometryTool.RayHitPlane(from, dir, behavior.getPlaneNormal(), behavior.getPlanePoint(), out hitPoint);
        var shereWorldCenter = controller.getSphereWorldCenter(); // 先射球
        var R = controller.getSphereR();
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

    Vector3 rayOnPlane(Vector3 from, Vector3 dir, SphereTerrain sTerrain)
    {
        Vector3 hitOnPlane = Vector3.zero;
        GeometryTool.RayHitPlane(from, dir, sTerrain.getPlaneNormal(), sTerrain.getPlanePoint(), out hitOnPlane);
        controller.hitOnPlane.position = hitOnPlane;

        return hitOnPlane;
    }
}
