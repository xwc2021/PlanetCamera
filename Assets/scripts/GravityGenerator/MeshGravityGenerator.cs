using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//非球圓形星球：像是Donuts、Knot
public class MeshGravityGenerator : MonoBehaviour, GrounGravityGenerator
{
    public bool averageSG = true;
    public float findingGravitySensorR = 4;
    public Transform findingGravitySphere=null;

    Collider[] gs = new Collider[100];//大小看需求自己設定
    public Vector3 findGroundUp()
    {
        int layerMask = 1 << 11;
        int overlapCount = Physics.OverlapSphereNonAlloc(transform.position, findingGravitySensorR, gs, layerMask);
        //Debug.DrawLine(transform.position, transform.position + groundUp * findingGravitySensorR);

        //print("overlapCount=" + overlapCount);

        if (overlapCount == 0)
            return transform.up;

        if (averageSG)
        {
            //找出nearestGS的平均值
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < overlapCount; i++)
            {
                Collider nowGS = gs[i];
                sum = sum + nowGS.transform.forward;
            }
            sum = sum / overlapCount;
            sum.Normalize();
            Debug.DrawLine(transform.position, transform.position + sum * 6, Color.green);
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
