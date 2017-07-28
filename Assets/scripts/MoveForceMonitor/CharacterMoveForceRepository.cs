using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GroundType { Normal,NormalLowGravity, Ice, Seesaw }

public class CharacterMoveForceRepository : MonoBehaviour
{
    MoveForceMonitor moveForceMonitor;
    public GroundType groundType = GroundType.Normal;
    public CharacterMoveForceMonitor normal;
    public CharacterMoveForceMonitor iceSkating;
    public CharacterMoveForceMonitor seesaw;
    public CharacterMoveForceMonitor normalLowGravity;//可以用來防止卡斜坡(斜度不能太大)

    public void resetGroundType(GroundType gType,Rigidbody rigid)
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

    public MoveForceMonitor getMoveForceMonitor()
    {
        return moveForceMonitor;
    }
}
