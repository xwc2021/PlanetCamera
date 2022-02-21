using System.Collections.Generic;
using UnityEngine;
//非球圓形星球：像是Donuts、Knot
public class MeshGravityGenerator : MonoBehaviour, GroundGravityGenerator
{


    public float findingGravitySensorR = 4;
    Collider[] colliderList = new Collider[100];//大小看需求自己設定
    Color purple = new Color(159.0f / 255, 90.0f / 255, 253.0f / 255);

    void Awake()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        triangles = mesh.triangles;

        vertices = mesh.vertices;
        normals = mesh.normals;
        createGravitySensor();
    }

    int[] triangles;
    Vector3[] vertices;
    Vector3[] normals;
    GravitySensor[] gsList;
    public GravitySensor gravitySensorPrefab; // Prefab
    void createGravitySensor()
    {
        var modelPos = transform.position;
        int vCount = vertices.Length;

        // 跑1次所有vertex
        gsList = new GravitySensor[vCount];
        for (int i = 0; i < vCount; i++)
        {
            var v0 = vertices[i];
            var n0 = normals[i];
            Quaternion rot0 = Quaternion.LookRotation(n0);
            var gs = Instantiate(gravitySensorPrefab, v0 + modelPos, rot0, this.transform);
            gs.init();
            gs.triangelIndex = i;
            gs.name = "gs" + i;
            gsList[i] = gs;
        }

        // 跑1次所有三角形
        int triCount = triangles.Length / 3;
        for (int i = 0; i < triCount; ++i)
        {
            int v0Index = triangles[3 * i];
            int v1Index = triangles[3 * i + 1];
            int v2Index = triangles[3 * i + 2];
            gsList[v0Index].addNeighborTriangleIndex(i);
            gsList[v1Index].addNeighborTriangleIndex(i);
            gsList[v2Index].addNeighborTriangleIndex(i);
        }
    }

    void createGravityDir(GravitySensor gs)
    {
        // 找出位在那個三角形上
        var triIndexList = gs.neighborTriangleIndex;
        // print(triIndexList.Count + "->" + gs.info());

        var triCount = triIndexList.Count;
        for (var i = 0; i < triCount; ++i)
        {
            var triIndex = triIndexList[i];
            int v0Index = triangles[3 * triIndex];
            int v1Index = triangles[3 * triIndex + 1];
            int v2Index = triangles[3 * triIndex + 2];
            print(v0Index + "," + v1Index + "," + v2Index);

            var v0 = vertices[v0Index];
            var v1 = vertices[v1Index];
            var v2 = vertices[v2Index];

            var parentPos = transform.position;
            v0 += parentPos;
            v1 += parentPos;
            v2 += parentPos;
            Debug.DrawLine(v0, v1, Color.red);
            Debug.DrawLine(v1, v2, Color.green);
            Debug.DrawLine(v2, v0, Color.blue);
        }
    }

    public Vector3 findGravityDir(Vector3 headUp, Vector3 targetPos, bool isHitFloor, Vector3 hitFloorPos)
    {
        // 收集GS
        int layerMask = 1 << LayerDefined.GravitySensor;
        int overlapCount = Physics.OverlapSphereNonAlloc(targetPos, findingGravitySensorR, colliderList, layerMask);
        print("overlapCount=" + overlapCount);
        if (overlapCount == 0)
            return -headUp;

        // 找出最近的GS
        Collider nearestC = null;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < overlapCount; i++)
        {
            Collider c = colliderList[i];
            float nowDistance = (c.transform.position - targetPos).sqrMagnitude;
            Debug.DrawRay(c.transform.position, c.transform.forward * 5);
            if (nowDistance < nearestDistance)
            {
                nearestC = c;
                nearestDistance = nowDistance;
            }
        }
        var gs = nearestC.GetComponent<GravitySensor>();
        createGravityDir(gs);

        Debug.DrawRay(nearestC.transform.position, nearestC.transform.forward * 5, purple);
        return -nearestC.transform.forward;
    }
}