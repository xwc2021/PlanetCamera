using UnityEngine;
//非球圓形星球：像是Donuts、Knot
public class MeshGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public bool averageSG = true;
    public float findingGravitySensorR = 4;

    Collider[] gs = new Collider[100];//大小看需求自己設定
    public Vector3 findGroundUp(Vector3 headUp, ref Vector3 targetPos)
    {
        int layerMask = 1 << LayerDefined.GravitySensor;
        int overlapCount = Physics.OverlapSphereNonAlloc(targetPos, findingGravitySensorR, gs, layerMask);
        print("overlapCount=" + overlapCount);
        if (overlapCount == 0)
            return headUp;
        //Debug.DrawLine(transform.position, transform.position + groundUp * findingGravitySensorR);

        if (averageSG)
        {
            //找出nearestGS的平均值
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < overlapCount; i++)
            {
                Collider nowGS = gs[i];
                Debug.DrawLine(nowGS.transform.position, nowGS.transform.position + nowGS.transform.forward * findingGravitySensorR);

                sum = sum + nowGS.transform.forward;
            }
            sum = sum / overlapCount;
            sum.Normalize();
            Debug.DrawLine(targetPos, targetPos + sum * 6, Color.green);
            return sum;
        }
        else
        {
            //找出最近的GS
            Collider nearestGS = null;
            float nearestDistance = float.MaxValue;
            Vector3 nowPos = transform.position;
            for (int i = 0; i < overlapCount; i++)
            {
                Collider nowGS = gs[i];
                Debug.DrawLine(nowGS.transform.position, nowGS.transform.position + nowGS.transform.forward * findingGravitySensorR);

                float nowDistance = (nowGS.transform.position - nowPos).sqrMagnitude;
                if (nowDistance < nearestDistance)
                {
                    nearestGS = nowGS;
                    nearestDistance = nowDistance;
                }
            }
            Debug.DrawLine(nearestGS.transform.position, nearestGS.transform.position + nearestGS.transform.forward * findingGravitySensorR);
            return nearestGS.transform.forward;
        }
    }
}