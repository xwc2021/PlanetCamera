using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasuringJumpHeight : MonoBehaviour
{

    bool recordHeight = false;
    Vector3 recordPosBeforeJump;
    public float height;

    public void startRecord()
    {
        height = 0;
        recordPosBeforeJump = transform.position;
        recordHeight = true;
    }

    public void stopRecord()
    {
        recordHeight = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (recordHeight)
        {
            height = Mathf.Max(Vector3.Dot((transform.position - recordPosBeforeJump), transform.up), height);
        }
    }
}
