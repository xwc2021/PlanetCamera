using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public PlanetPlayerController planetPlayerController;

    public float moveForceOnAirLow = 20;
    public float moveForceOnAirHight = 20;
    public float moveForceLow = 20;
    public float moveForceHight = 35;
    public float gravity = 10;
    public float gravityOnAir = 40;

    public float moveForceLowNormal = 20;
    public float moveForceHightNormal = 35;
    public float gravityNormal = 10;
    public float gravityOnAirNormal = 40;
    public float dragNormal = 4;

    public float moveForceLowIce = 20;
    public float moveForceHightIce = 35;
    public float gravityIce = 10;
    public float gravityOnAirIce = 40;
    public float dragIce = 0.5f;

    public float moveForceLowSeesaw = 30;
    public float moveForceHightSeesaw = 45;
    public float gravitySeesaw = 40;
    public float gravityOnAirSeesaw = 40;
    public float dragSeesaw = 4;

    public float getMoveForceStrength(bool isOnAir)
    {
        if (!isOnAir)
        {
            //按住fire鈕才加速
            if (planetPlayerController.holdFire())
                return moveForceHight;
            else
                return moveForceLow;
        }
        else
        {
            //如果在空中時的移動加速度和在地面時一樣，會感覺在空中偏快
            if (planetPlayerController.holdFire())
                return moveForceOnAirHight;
            else
                return moveForceOnAirLow;
        }
        
    }

    public float getGravityForceStrength(bool isOnAir)
    {
        if (isOnAir)
            return gravityOnAir;
        else
            return gravity;
    }

    public void enableNormal(Rigidbody rigid)
    {
        rigid.drag = dragNormal;

        moveForceLow = moveForceLowNormal;
        moveForceHight = moveForceHightNormal;

        gravity = gravityNormal;
        gravityOnAir = gravityOnAirNormal;
    }

    public void enableIceSkating(Rigidbody rigid)
    {
        rigid.drag = dragIce;

        moveForceLow = moveForceLowIce;
        moveForceHight = moveForceHightIce;

        gravity = gravityIce;
        gravityOnAir = gravityOnAirIce;
    }

    public void enableSeesaw(Rigidbody rigid)
    {
        rigid.drag = dragSeesaw;

        moveForceLow = moveForceLowSeesaw;
        moveForceHight = moveForceHightSeesaw;

        gravity = gravitySeesaw;
        gravityOnAir = gravityOnAirSeesaw;
    }
}
