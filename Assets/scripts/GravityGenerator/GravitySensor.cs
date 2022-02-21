using System.Collections.Generic;
using UnityEngine;
public class GravitySensor : MonoBehaviour
{
    public List<int> neighborTriangleIndex;
    public void init() { neighborTriangleIndex = new List<int>(); }
    public void addNeighborTriangleIndex(int index)
    {
        neighborTriangleIndex.Add(index);
    }
    public int triangelIndex;
    public string info()
    {
        var len = neighborTriangleIndex.Count;
        var msg = "";
        for (var i = 0; i < len; ++i)
            msg += neighborTriangleIndex[i] + ",";

        return msg;
    }
}