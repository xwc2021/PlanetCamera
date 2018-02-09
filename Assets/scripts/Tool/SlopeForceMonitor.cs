using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeForceMonitor : MonoBehaviour {

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
    public Vector3 modifyMoveForce(Vector3 moveForce,float moveForceStrength, float GravityForceStrength, Vector3 groundUp, Vector3 SlopeUp)
    {
        Vector3 fAlongSlope = getPartOfGravityForceStrengthAlongSlope(GravityForceStrength, groundUp, SlopeUp);
        Vector3 moveForceWithStrength = moveForce * moveForceStrength;

        //這裡又作了一次ProjectOnPlane的動作，原因是
        //fAlongSlope會和moveForceWithStrengthALongSlop方向平行
        //但是玩家在斜坡上移動時，未必只會況了moveForceWithStrengthALongSlop方向移動
        //https://plus.google.com/u/0/+XiangweiChiou/posts/cLCHt28TaXq
        Vector3 planeNormal = Vector3.Cross(groundUp, SlopeUp).normalized;
        Vector3 moveForceWithStrengthALongSlop =Vector3.ProjectOnPlane(moveForceWithStrength, planeNormal);

        float dotValue =Vector3.Dot(fAlongSlope, moveForceWithStrengthALongSlop);
        bool sameDir = dotValue > 0;
        float sign = sameDir ? -1 : 1;

        //moveForceWithStrength可以拆成2個力
        //和fAlongSlope同方向的力:moveForceWithStrengthALongSlop
        //和fAlongSlope垂直的力:moveForceHorizontal
        //moveForceWithStrength = moveForceWithStrengthALongSlop + moveForceHorizontal
        //當上坡時, 玩家沿著fAlongSlope受的合力 = moveForceWithStrengthALongSlop-fAlongSlopeStrength - 摩擦力(重力垂直於斜坡的分力)
        //當下坡時, 玩家沿著fAlongSlope受的合力 = moveForceWithStrengthALongSlop+fAlongSlopeStrength - 摩擦力(重力垂直於斜坡的分力)
        //所以才會發現上坡比較慢，下坡比較快

        //摩擦力
        //http://home.phy.ntnu.edu.tw/~eureka/contents/elementary/chap%202/2-4-1.htm

        //為了讓上坡下坡和在地面時差不多，只要抵消掉fAlongSlope這項就行了
        //sign是跟moveForceWithStrengthALongSlop變化，而不是fAlongSlope(所以不是浮點誤差的關系)
        //Vector3 tempFinalMoveForce = moveForceWithStrength - sign * fAlongSlope;
        Vector3 finalMoveForce = moveForceWithStrength + sign * fAlongSlope.magnitude* moveForceWithStrengthALongSlop.normalized;

        float limitSpeed =Mathf.Min(finalMoveForce.magnitude, maxForceLimit);
        finalMoveForce = finalMoveForce.normalized * limitSpeed;

        //Debug.DrawRay(transform.position , SlopeUp, Color.yellow);
        //Debug.DrawRay(transform.position, fAlongSlope, Color.red);
        //Debug.DrawRay(transform.position+groundUp, moveForceWithStrengthALongSlop, Color.green);
        //print("上坡:"+!sameDir + " ForceAlongSlope=" + fAlongSlope.magnitude+" moveForce:" +moveForceStrength + " modifyMoveForce:" + finalMoveForce.magnitude);
        return finalMoveForce;
    }
}
