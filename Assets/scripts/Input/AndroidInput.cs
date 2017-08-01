using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AndroidInput : MonoBehaviour, InputProxy
{
    bool isFire;
    bool isJump;
    Vector2 memoryPos;
    Vector2 nowInput;
    Vector2 doAndroidInput()
    {
        if (Input.touchCount == 1)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                memoryPos = Input.touches[0].position;
            }

            if (Input.touches[0].phase == TouchPhase.Moved && Input.touches[0].phase != TouchPhase.Canceled)
            {

                nowInput = Input.touches[0].position - memoryPos;
                //nowInput = Input.touches[0].deltaPosition;
            }

            else if (Input.touches[0].phase == TouchPhase.Ended)
                nowInput = new Vector2(0, 0);
        }

        return nowInput;

    }

    Vector2 InputProxy.getHV()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        return new Vector2(h, v);
    }

    bool InputProxy.pressJump()
    {
        if (isJump)
        {
            isJump = false;
            return true;
        }
        else
            return false;
    }

    bool InputProxy.pressFire()
    {
        return false;
    }

    bool InputProxy.holdFire()
    {
        return isFire;
    }

    public void onClickJumpButton()
    {
        isJump = true;
    }

    public void onClickFireButton()
    {
        isFire = true;
    }

    public void onClickFireButtonUp()
    {
        isFire = false;
    }
}
