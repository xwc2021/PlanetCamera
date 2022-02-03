﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeForceMonitor : MonoBehaviour
{

    public Vector3 getPartOfGravityForceStrengthAlongSlope(float GravityForceStrength, Vector3 groundUp, Vector3 SlopeUp)
    {
        //重力 = 垂直於斜坡的分力+沿著斜坡的分力
        float cosValue = Vector3.Dot(groundUp, SlopeUp); // 和Dot(-groundUp, -SlopeUp)結果一樣
        float perpendicularStrength = GravityForceStrength * cosValue;

        Vector3 f = GravityForceStrength * (-groundUp) - perpendicularStrength * (-SlopeUp);
        //print("getPartOfGravityForceStrengthAlongSlope="+f.magnitude);
        return f;
    }

    public float maxForceLimit = 120;
    public Vector3 modifyMoveForce(Vector3 moveForceWithStrength, float GravityForceStrength, Vector3 groundUp, Vector3 SlopeUp)
    {
        // https://photos.app.goo.gl/ZPEjsEX5XryktCBv8
        // fAlongSlopeStrength 是重力沿著斜坡的分力
        var fAlongSlope = getPartOfGravityForceStrengthAlongSlope(GravityForceStrength, groundUp, SlopeUp);

        // moveForceWithStrength可以拆成2個力
        // moveForceWithStrength = moveForceWithStrengthALongSlop + moveForceHorizontal
        var slopN = fAlongSlope.normalized;
        var cosValue = Vector3.Dot(slopN, moveForceWithStrength);
        var moveForceWithStrengthALongSlop = slopN * cosValue;

        bool isMoveDown = cosValue > 0;

        // 玩家沿著fAlongSlope受的合力 = moveForceWithStrengthALongSlop + fAlongSlopeStrength - 摩擦力(重力垂直於斜坡的分力)
        // 上坡時，fAlongSlopeStrength會讓玩家減速
        // 下坡時，fAlongSlopeStrength會讓玩家加速
        // 為了讓上坡下坡速度固定，要想辦法抵消fAlongSlopeStrength
        Vector3 finalMoveForce = moveForceWithStrength - fAlongSlope;

        float limitSpeed = Mathf.Min(finalMoveForce.magnitude, maxForceLimit);
        finalMoveForce = finalMoveForce.normalized * limitSpeed;

        // Debug.DrawRay(transform.position, SlopeUp, Color.yellow);
        // Debug.DrawRay(transform.position, fAlongSlope, Color.red);
        // Debug.DrawRay(transform.position + groundUp, moveForceWithStrengthALongSlop, Color.green);
        //print("上坡:"+!sameDir + " ForceAlongSlope=" + fAlongSlope.magnitude+" moveForce:" +moveForceStrength + " modifyMoveForce:" + finalMoveForce.magnitude);
        return finalMoveForce;
    }
}
