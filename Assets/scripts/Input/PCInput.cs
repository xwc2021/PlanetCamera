using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PCInput : MonoBehaviour,  InputProxy
{
    public bool pressJump()
    {
        return CrossPlatformInputManager.GetButtonDown("Jump");
    }

    public Vector2 getHV()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        return new Vector2(h, v);
    }

    public bool pressFire()
    {
        return CrossPlatformInputManager.GetButtonDown("Fire1");
    }

    public bool holdFire()
    {
        return CrossPlatformInputManager.GetButton("Fire1");
    }
}
