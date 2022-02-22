using System.Collections.Generic;
using UnityEngine;
//非球圓形星球：像是Donuts、Knot
public class MeshGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
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

    Vector3 getInterpolationNormal(List<int> triIndexList, ref Vector3 movablePos, ref Vector3 headUp)
    {
        var triCount = triIndexList.Count;
        for (var i = 0; i < triCount; ++i)
        {
            var triIndex = triIndexList[i];
            int v0Index = triangles[3 * triIndex];
            int v1Index = triangles[3 * triIndex + 1];
            int v2Index = triangles[3 * triIndex + 2];

            var v0 = vertices[v0Index];
            var v1 = vertices[v1Index];
            var v2 = vertices[v2Index];

            var parentPos = transform.position;
            v0 += parentPos;
            v1 += parentPos;
            v2 += parentPos;
            var N = GeometryTool.CalculateTriangleNormal(ref v0, ref v1, ref v2);

            // 擊中三角形所在的平面
            Vector3 hitPos;
            if (!GeometryTool.RayHitPlane(movablePos, -N, N, v0, out hitPos))
                continue;

            bool isGetValue;
            float a, b, r;
            GeometryTool.CalculateBarycentricCoordinates(ref v0, ref v1, ref v2, ref hitPos, out isGetValue, out a, out b, out r);
            if (!isGetValue)
                continue;

            if (!GeometryTool.isInTriangle(a, b, r))
                continue;

            Debug.DrawLine(v0, v1, Color.red);
            Debug.DrawLine(v1, v2, Color.green);
            Debug.DrawLine(v2, v0, Color.blue);

            var n0 = normals[v0Index];
            var n1 = normals[v1Index];
            var n2 = normals[v2Index];

            Debug.DrawRay(v0, n0 * 5);
            Debug.DrawRay(v1, n1 * 5);
            Debug.DrawRay(v2, n2 * 5);

            var normal = GeometryTool.CalculateInterpolationValueByBarycentricCoordinates(ref n0, ref n1, ref n2, a, b, r);
            return normal.normalized;
        }
        return headUp;
    }

    public bool usingInterpolationNormal = true;
    public float findingGravitySensorR = 4;
    Collider[] colliderList = new Collider[100];//大小看需求自己設定
    Color purple = new Color(159.0f / 255, 90.0f / 255, 253.0f / 255);

    public Vector3 findGravityDir(Vector3 headUp, Vector3 movablePos, bool isHitFloor, Vector3 hitFloorPos)
    {
        // 收集GS
        int layerMask = 1 << LayerDefined.GravitySensor;
        int overlapCount = Physics.OverlapSphereNonAlloc(movablePos, findingGravitySensorR, colliderList, layerMask);
        // print("overlapCount=" + overlapCount);
        if (overlapCount == 0)
            return -headUp;

        Collider nearestC;
        var allNeighborTriangelIndex = getAllNeighborTriangelIndex(ref movablePos, overlapCount, out nearestC);
        var normal = getInterpolationNormal(allNeighborTriangelIndex, ref movablePos, ref headUp);
        Debug.DrawRay(nearestC.transform.position, nearestC.transform.forward * 5, purple);
        Debug.DrawRay(movablePos, normal * 10, Color.black);
        if (usingInterpolationNormal)
            return -normal;
        else
            return -nearestC.transform.forward;
    }

    List<int> getAllNeighborTriangelIndex(ref Vector3 movablePos, int overlapCount, out Collider nearestC)
    {
        // 只有1個的話
        if (overlapCount == 1)
        {
            var c = colliderList[0];
            nearestC = c;
            return c.GetComponent<GravitySensor>().neighborTriangleIndex;
        }

        // 找出最近的GS(1個以上)
        // 有甜甜圈的交界處的GravitySensor，會漏掉一些相鄰資訊
        var vertexInfoList = new List<VertexInfo>();
        for (int i = 0; i < overlapCount; i++)
        {
            Collider c = colliderList[i];
            var vPos = c.transform.position;
            var vNormal = c.transform.forward;
            vertexInfoList.Add(new VertexInfo() { position = vPos, normal = vNormal, distance = (vPos - movablePos).sqrMagnitude, collider = c });
        }

        vertexInfoList.Sort(
            (VertexInfo a, VertexInfo b) =>
            {
                if (a.distance < b.distance) return -1;
                else return 1;
            }
        );

        nearestC = vertexInfoList[0].collider;

        // 最近的2個
        var gs0 = vertexInfoList[0].collider.GetComponent<GravitySensor>();
        var gs1 = vertexInfoList[1].collider.GetComponent<GravitySensor>();
        var distance_0_1 = (gs0.transform.position - gs1.transform.position).magnitude;
        if (GeometryTool.floatEqual(distance_0_1, 0))
        {
            // Debug.Log("GravitySensor合體");
            List<int> list = new List<int>();
            list.AddRange(gs0.neighborTriangleIndex);
            list.AddRange(gs1.neighborTriangleIndex);
            return list;
        }
        else
            return gs0.neighborTriangleIndex;
    }

    struct VertexInfo
    {
        public Vector3 position;
        public Vector3 normal;
        public float distance;
        public Collider collider;
    }
}