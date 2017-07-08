using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public PlanetPlayerController planetPlayerController;
    public float moveForceScaleLow = 80;
    public float moveForceScaleHight = 110;
    public float getMoveForceStrength()
    {
        //按住fire鈕才加速
        if(planetPlayerController.holdFire())
            return moveForceScaleHight;
        else
            return moveForceScaleLow;
    }
}
