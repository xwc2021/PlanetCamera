using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTerrainBrushController : MonoBehaviour
{
    public float brushStrength = 0.1f;
    public float brushSize = 10.0f;
    public bool clear = false;
    void Update()
    {
        foreach (var s in sphereTerrains)
        {
            s.updateBrushStrength(brushStrength);
            s.updateBrushSzie(brushSize);

            s.clearHeight(clear);
        }
    }

    public Transform from;
    public Transform to;
    public Transform hitOnPlane;

    public SphereTerrain[] sphereTerrains;
    static SphereTerrain[] testingTerrains;
    public void getTestingTerrains(Vector3 v, out SphereTerrain[] testingList, out int testingListCount)
    {
        if (testingTerrains == null)
            testingTerrains = new SphereTerrain[6];

        var index = 0;
        var threshold = Mathf.Cos(120.0f * Mathf.Deg2Rad);
        for (var i = 0; i < sphereTerrains.Length; ++i)
        {
            var sTerrain = sphereTerrains[i];
            var cosValue = Vector3.Dot(v, sTerrain.transform.up);
            // if (cosValue > threshold)
            {
                testingTerrains[index] = sTerrain;
                ++index;
            }
        }

        testingList = testingTerrains;
        testingListCount = index;

        var str = threshold + " ";
        for (var i = 0; i < testingListCount; ++i)
            str += testingList[i].name + ",";
        print(str);
    }

    public void setBrushLocalPos(Vector3 hitPointWorld)
    {
        foreach (var sTerrain in sphereTerrains)
            sTerrain.setBrushLocalPos(hitPointWorld);
    }

    public void useBrush(bool value)
    {
        foreach (var sTerrain in sphereTerrains)
            sTerrain.useBrush(value);
    }

    public float getSphereR()
    {
        return 511.5f;
    }

    public Vector3 getSphereWorldCenter()
    {
        return transform.position;
    }
}
