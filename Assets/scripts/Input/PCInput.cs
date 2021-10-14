using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PCInput : InputProxy
{
    bool InputProxy.pressJump()
    {
        return CrossPlatformInputManager.GetButtonDown("Jump");
    }

    Vector2 InputProxy.getHV()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        return new Vector2(h, v);
    }

    bool InputProxy.pressFire()
    {
        return CrossPlatformInputManager.GetButtonDown("Fire1");
    }

    bool InputProxy.holdFire()
    {
        return CrossPlatformInputManager.GetButton("Fire1");
    }

    float scale = 1;
    float InputProxy.yawScale()
    {
        return scale;
    }

    float InputProxy.pitchScale()
    {
        return scale;
    }

    bool InputProxy.enableControlUI()
    {
        return false;
    }
}
