using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public string name="";
    public float jumpForceLow = 1500;
    public float jumpForceHight = 1700;

    public float moveForceOnAirLow = 20;
    public float moveForceOnAirHight = 20;

    public float moveForceLow = 20;
    public float moveForceHight = 35;

    public float gravity = 10;
    public float gravityOnAir = 40;

    public float drag = 4;

    float MoveForceMonitor.getMoveForceStrength(bool isOnAir,bool isTurbo)
    {
        if (!isOnAir)
        {
            //按住fire鈕才加速
            if (isTurbo)
                return moveForceHight;
            else
                return moveForceLow;
        }
        else
        {
            //如果在空中時的移動加速度和在地面時一樣，會感覺在空中偏快
            if (isTurbo)
                return moveForceOnAirHight;
            else
                return moveForceOnAirLow;
        }
        
    }

    float MoveForceMonitor.getGravityForceStrength(bool isOnAir)
    {
        if (isOnAir)
            return gravityOnAir;
        else
            return gravity;
    }

    float MoveForceMonitor.getJumpForceStrength(bool isTurbe)
    {
        //按住fire鈕才加速
        if (isTurbe)
            return jumpForceHight;
        else
            return jumpForceLow;
    }

    void MoveForceMonitor.setRigidbodyParamter(Rigidbody rigid)
    {
        rigid.drag = drag;
    }
}
