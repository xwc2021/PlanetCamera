using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpForceMonitor : MonoBehaviour,JumpForceMonitor {

    public PlanetPlayerController planetPlayerController;
    static float jumpForceScaleLow = 1200;
    static float jumpForceScaleHight = 2000;
    public float getJumpForceStrength()
    {
        //按住fire鈕才加速
        if (planetPlayerController.holdFire())
            return jumpForceScaleHight;
        else
            return jumpForceScaleLow;
    }
}
