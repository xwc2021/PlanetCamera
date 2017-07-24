using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpForceMonitor : MonoBehaviour,JumpForceMonitor {

    public PlanetPlayerController planetPlayerController;
    public float jumpForceScaleLow = 1500;
    public float jumpForceScaleHight = 1700;
    public float getJumpForceStrength()
    {
        //按住fire鈕才加速
        if (planetPlayerController.holdFire())
            return jumpForceScaleHight;
        else
            return jumpForceScaleLow;
    }
}
