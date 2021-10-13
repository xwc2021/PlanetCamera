using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstance
{
    // 之後再來試試 為什麼直接呼叫Graphics.DrawMeshInstanced沒效果
    static List<Matrix4x4>[] matrixList;
    static List<Transform> transformList = new List<Transform>();
    static bool is_static = false;
    public static void pushTrasform(Transform t)
    {
        DrawInstance.transformList.Add(t);
    }

    public static void removeTrasform(Transform t)
    {
        DrawInstance.transformList.Remove(t);
    }

    public static void initMatrix(int draw_count, bool is_static)
    {
        // static 只會更新 matrix 1次
        DrawInstance.is_static = is_static;

        var batch = (draw_count / 1023) + 1;
        DrawInstance.matrixList = new List<Matrix4x4>[batch];
        for (var i = 0; i < batch; ++i)
            DrawInstance.matrixList[i] = new List<Matrix4x4>();
    }

    public static void updateMatrix(int from, int to, List<Matrix4x4> list)
    {
        var max = DrawInstance.transformList.Count;
        list.Clear();
        var test_m = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 10, 0, 1));
        for (var i = from; i < to; ++i)
        {
            if (i >= max) // 超過了
                break;

            var m = DrawInstance.transformList[i].localToWorldMatrix;
            list.Add(m);
        }
    }

    public static bool is_dirty = true;
    public static void draw(Mesh instanceMesh, Material[] instanceMaterial)
    {
        var count = DrawInstance.transformList.Count;
        var batch_count = (count / 1023) + 1;
        var len = 1023;
        var from = 0;
        var to = len;
        for (var i = 0; i < batch_count; ++i)
        {
            if (!DrawInstance.is_static || (DrawInstance.is_static && DrawInstance.is_dirty))
                DrawInstance.updateMatrix(from, to, DrawInstance.matrixList[i]);

            var m_count = instanceMaterial.Length;
            for (var j = 0; j < m_count; ++j)
                Graphics.DrawMeshInstanced(instanceMesh, j, instanceMaterial[j], DrawInstance.matrixList[i]);

            from += len;
            to += len;
        }

        DrawInstance.is_dirty = false;
    }
}
