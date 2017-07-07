using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public float moveForceScale = 110f;
    public float getNowForceStrength()
    {
        return moveForceScale;
    }
}
