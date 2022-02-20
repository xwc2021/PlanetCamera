using UnityEngine;
//非球圓形星球：像是Donuts、Knot
public class MeshGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public float findingGravitySensorR = 4;

    Collider[] gs = new Collider[100];//大小看需求自己設定
    Color purple = new Color(159.0f / 255, 90.0f / 255, 253.0f / 255);

    public Vector3 findGravityDir(Vector3 headUp, Vector3 targetPos)
    {
        // 收集GS
        int layerMask = 1 << LayerDefined.GravitySensor;
        int overlapCount = Physics.OverlapSphereNonAlloc(targetPos, findingGravitySensorR, gs, layerMask);
        print("overlapCount=" + overlapCount);
        if (overlapCount == 0)
            return -headUp;

        // 找出最近的GS
        Collider nearestGS = null;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < overlapCount; i++)
        {
            Collider nowGS = gs[i];
            float nowDistance = (nowGS.transform.position - targetPos).sqrMagnitude;
            Debug.DrawRay(nowGS.transform.position, nowGS.transform.forward * nowDistance);
            if (nowDistance < nearestDistance)
            {
                nearestGS = nowGS;
                nearestDistance = nowDistance;
            }
        }

        Debug.DrawRay(nearestGS.transform.position, nearestGS.transform.forward * nearestDistance, purple);
        return -nearestGS.transform.forward;
    }
}