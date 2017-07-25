using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GroundType { Normal,NormalLowGravity, Ice, Seesaw }

public class PlayerMoveForceRepository : MonoBehaviour,MoveForceMonitor, MoveForceSelector
{
    MoveForceMonitor moveForceMonitor;
    public GroundType groundType = GroundType.Normal;
    public PlayerMoveForceMonitor normal;
    public PlayerMoveForceMonitor iceSkating;
    public PlayerMoveForceMonitor seesaw;
    public PlayerMoveForceMonitor normalLowGravity;//可以用來防止卡斜坡(斜度不能太大)

    void MoveForceSelector.resetByGroundType(GroundType gType,Rigidbody rigid)
    {
        groundType = gType;
        switch (groundType)
        {
            case GroundType.Normal:
                moveForceMonitor = normal as MoveForceMonitor;
                break;
            case GroundType.NormalLowGravity:
                moveForceMonitor = normalLowGravity as MoveForceMonitor;
                break;
            case GroundType.Ice:
                moveForceMonitor = iceSkating as MoveForceMonitor;
                break;
            case GroundType.Seesaw:
                moveForceMonitor = seesaw as MoveForceMonitor;
                break;
        }

        moveForceMonitor.setRigidbodyParamter(rigid);
    }

    float MoveForceMonitor.getGravityForceStrength(bool isOnAir)
    {
        return moveForceMonitor.getGravityForceStrength(isOnAir);
    }

    float MoveForceMonitor.getJumpForceStrength(bool isTurble)
    {
        return moveForceMonitor.getJumpForceStrength(isTurble);
    }

    float MoveForceMonitor.getMoveForceStrength(bool isOnAir, bool isTurble)
    {
        return moveForceMonitor.getMoveForceStrength(isOnAir, isTurble);
    }

    void MoveForceMonitor.setRigidbodyParamter(Rigidbody rigid)
    {
        moveForceMonitor.setRigidbodyParamter(rigid);
    }
}
