using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public float moveForceScale = 120f;
    public float getNowForceStrength()
    {
        return moveForceScale;
    }
}
