using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidInput : MonoBehaviour, InputProxy
{
    public Vector2 getHV()
    {
        return doAndroidInput();
    }

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

}
