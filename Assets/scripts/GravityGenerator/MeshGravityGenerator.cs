using UnityEngine;
//非球圓形星球：像是Donuts、Knot
public class MeshGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public float findingGravitySensorR = 4;
    public Transform gs;

    Collider[] colliderList = new Collider[100];//大小看需求自己設定
    Color purple = new Color(159.0f / 255, 90.0f / 255, 253.0f / 255);

    void Awake()
    {
        createGS();
    }

    public void createGS()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        var modelPos = transform.position;
        int vCount = mesh.vertices.Length;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < vCount; i++)
        {
            var v0 = vertices[i];
            var n0 = normals[i];
            Quaternion rot0 = Quaternion.LookRotation(n0);
            Instantiate(gs, v0 + modelPos, rot0, this.transform);
        }
    }

    public Vector3 findGravityDir(Vector3 headUp, Vector3 targetPos)
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
            Debug.DrawRay(c.transform.position, c.transform.forward * nowDistance);
            if (nowDistance < nearestDistance)
            {
                nearestC = c;
                nearestDistance = nowDistance;
            }
        }

        Debug.DrawRay(nearestC.transform.position, nearestC.transform.forward * nearestDistance, purple);
        return -nearestC.transform.forward;
    }
}