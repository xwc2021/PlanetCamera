using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstance
{
    // 之後再來試試 為什麼直接呼叫Graphics.DrawMeshInstanced沒效果
    static List<Matrix4x4> matrixList;
    static List<Transform> transformList = new List<Transform>();
    public static void pushTrasform(Transform t)
    {
        DrawInstance.transformList.Add(t);
    }

    public static void removeTrasform(Transform t)
    {
        DrawInstance.transformList.Remove(t);
    }

    public static void initMatrix()
    {
        DrawInstance.matrixList = new List<Matrix4x4>(DrawInstance.transformList.Count);
    }

    public static void updateMatrix()
    {
        DrawInstance.matrixList.Clear();
        var count = DrawInstance.transformList.Count;
        var test_m = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 10, 0, 1));
        for (var i = 0; i < count; ++i)
        {
            var m = DrawInstance.transformList[i].localToWorldMatrix;
            DrawInstance.matrixList.Add(m);
        }
    }

    public static void draw(Mesh instanceMesh, Material[] instanceMaterial)
    {
        // Debug.Log(DrawInstanceBike.matrixList.Count);
        var count = instanceMaterial.Length;
        for (var i = 0; i < count; ++i)
            Graphics.DrawMeshInstanced(instanceMesh, i, instanceMaterial[i], DrawInstance.matrixList);
    }
}
