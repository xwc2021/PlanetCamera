using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public PlanetPlayerController planetPlayerController;

    public float moveForceScaleOnAirLow = 60;
    public float moveForceScaleOnAirHight = 60;

    public float moveForceScaleLow = 80;
    public float moveForceScaleHight = 90;
    public float getMoveForceStrength(bool isOnAir)
    {
        if (!isOnAir)
        {
            //按住fire鈕才加速
            if (planetPlayerController.holdFire())
                return moveForceScaleHight;
            else
                return moveForceScaleLow;
        }
        else
        {
            //如果在空中時的移動加速度和在地面時一樣，會感覺在空中偏快
            if (planetPlayerController.holdFire())
                return moveForceScaleOnAirHight;
            else
                return moveForceScaleOnAirLow;
        }
        
    }
}
