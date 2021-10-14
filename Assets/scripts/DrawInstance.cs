using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstance
{
    public static string KEY_bike = "KEY_bike";
    public static string KEY_water_park_block = "KEY_water_park_block";

    static Dictionary<string, DrawInstance> dic = new Dictionary<string, DrawInstance>();
    public static DrawInstance getWorker(string key)
    {
        if (!dic.ContainsKey(key))
        {
            var worker = new DrawInstance();
            dic.Add(key, worker);
            return worker;
        }
        else
        {
            return dic[key];
        }
    }

    // 之後再來試試 為什麼直接呼叫Graphics.DrawMeshInstanced沒效果
    List<Matrix4x4>[] matrixList;
    List<Transform> transformList = new List<Transform>();
    bool is_static = false;

    public int getCount()
    {
        return this.transformList.Count;
    }

    public void pushTrasform(Transform t)
    {
        transformList.Add(t);
    }

    public void removeTrasform(Transform t)
    {
        transformList.Remove(t);
    }

    public void initMatrix(int draw_count, bool is_static)
    {
        // static 只會更新 matrix 1次
        this.is_static = is_static;

        var batch = (draw_count / 1023) + 1;
        matrixList = new List<Matrix4x4>[batch];
        for (var i = 0; i < batch; ++i)
            matrixList[i] = new List<Matrix4x4>();
    }

    public void updateMatrix(int from, int to, List<Matrix4x4> list)
    {
        var max = transformList.Count;
        list.Clear();
        var test_m = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 10, 0, 1));
        for (var i = from; i < to; ++i)
        {
            if (i >= max) // 超過了
                break;

            var m = transformList[i].localToWorldMatrix;
            list.Add(m);
        }
    }

    public bool is_dirty = true;
    public void draw(Mesh instanceMesh, Material[] instanceMaterial)
    {
        var count = transformList.Count;
        var batch_count = (count / 1023) + 1;
        var len = 1023;
        var from = 0;
        var to = len;
        for (var i = 0; i < batch_count; ++i)
        {
            if (!is_static || (is_static && is_dirty))
                updateMatrix(from, to, matrixList[i]);

            var m_count = instanceMaterial.Length;
            for (var j = 0; j < m_count; ++j)
                Graphics.DrawMeshInstanced(instanceMesh, j, instanceMaterial[j], matrixList[i]);

            from += len;
            to += len;
        }

        is_dirty = false;
    }
}
