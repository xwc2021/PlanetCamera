using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeForceMonitor : MonoBehaviour
{

    public Vector3 getPartOfGravityForceStrengthAlongSlope(float GravityForceStrength, Vector3 groundUp, Vector3 SlopeUp)
    {
        //重力 = 垂直於斜坡的分力+沿著斜坡的分力
        float cosValue = Vector3.Dot(groundUp, SlopeUp);
        float perpendicularStrength = GravityForceStrength * cosValue;

        Vector3 f = GravityForceStrength * (-groundUp) - perpendicularStrength * (-SlopeUp);
        //print("getPartOfGravityForceStrengthAlongSlope="+f.magnitude);
        return f;
    }

    public float maxForceLimit = 120;
    public Vector3 modifyMoveForce(Vector3 moveForceWithStrength, float GravityForceStrength, Vector3 groundUp, Vector3 SlopeUp)
    {
        // https://photos.app.goo.gl/ZPEjsEX5XryktCBv8
        // moveForceWithStrength可以拆成2個力
        // moveForceWithStrength = moveForceWithStrengthALongSlop + moveForceHorizontal

        // 找出moveForceWithStrengthALongSlop
        Vector3 planeTangentALongSlop = Vector3.Cross(groundUp, SlopeUp).normalized;
        Vector3 moveForceWithStrengthALongSlop = Vector3.ProjectOnPlane(moveForceWithStrength, planeTangentALongSlop);

        // fAlongSlopeStrength 是重力沿著斜坡的分力
        Vector3 fAlongSlope = getPartOfGravityForceStrengthAlongSlope(GravityForceStrength, groundUp, SlopeUp);
        float dotValue = Vector3.Dot(fAlongSlope, moveForceWithStrengthALongSlop);
        bool sameDir = dotValue > 0;
        float sign = sameDir ? -1 : 1;

        // 當上坡時, 玩家沿著fAlongSlope受的合力 = moveForceWithStrengthALongSlop - fAlongSlopeStrength - 摩擦力(重力垂直於斜坡的分力)
        // 當下坡時, 玩家沿著fAlongSlope受的合力 = moveForceWithStrengthALongSlop + fAlongSlopeStrength - 摩擦力(重力垂直於斜坡的分力)
        // 所以才會發現上坡比較慢，下坡比較快
        // 為了讓上坡下坡和在地面時差不多，只要抵消掉fAlongSlope這項就行了

        // 不知道是不是浮點誤差，有時fAlongSlope和moveForceWithStrengthALongSlop並沒有平行
        // Vector3 finalMoveForce = moveForceWithStrength - sign * fAlongSlope;
        Vector3 finalMoveForce = moveForceWithStrength + sign * fAlongSlope.magnitude * moveForceWithStrengthALongSlop.normalized;

        float limitSpeed = Mathf.Min(finalMoveForce.magnitude, maxForceLimit);
        finalMoveForce = finalMoveForce.normalized * limitSpeed;

        // Debug.DrawRay(transform.position, SlopeUp, Color.yellow);
        // Debug.DrawRay(transform.position, fAlongSlope, Color.red);
        // Debug.DrawRay(transform.position + groundUp, moveForceWithStrengthALongSlop, Color.green);
        //print("上坡:"+!sameDir + " ForceAlongSlope=" + fAlongSlope.magnitude+" moveForce:" +moveForceStrength + " modifyMoveForce:" + finalMoveForce.magnitude);
        return finalMoveForce;
    }
}
