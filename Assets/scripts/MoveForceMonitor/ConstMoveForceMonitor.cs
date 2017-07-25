using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public float moveForceScale = 110f;

    float MoveForceMonitor.getMoveForceStrength(bool isOnAir, bool isTurbo)
    {
        return moveForceScale;
    }

    float MoveForceMonitor.getGravityForceStrength(bool isOnAir)
    {
        return 0;
}

    float MoveForceMonitor.getJumpForceStrength(bool isTurble)
    {
        return 0;
    }

    void MoveForceMonitor.setRigidbodyParamter(Rigidbody rigid)
    {
        throw new NotImplementedException();
    }
}
