using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public float moveForceScale = 110f;
    public float getMoveForceStrength(bool isOnAir)
    {
        return moveForceScale;
    }

    public float getGravityForceStrength(bool isOnAir)
    {
        return 0;
    }

    public void enableNormal(Rigidbody rigid)
    {
        throw new NotImplementedException();
    }

    public void enableIceSkating(Rigidbody rigid)
    {
        throw new NotImplementedException();
    }

    public void enableSeesaw(Rigidbody rigid)
    {
        throw new NotImplementedException();
    }
}
