using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerMoveForceMonitor : MonoBehaviour, MoveForceMonitor
{
    public float minDist = 0.5f;
    public float maxDist = 10.0f;
    public float minForce = 120f;
    public float maxForce = 160f;
    public Transform followTarget;
    float MoveForceMonitor.getMoveForceStrength(bool isOnAir, bool isTurble)
    {
        if (followTarget == null)
            return 0;

        float diff = (followTarget.transform.position - this.transform.position).magnitude;
        float t = diff / (maxDist - minDist);
        return Mathf.Lerp(minForce, maxForce, t);
    }

    float MoveForceMonitor.getGravityForceStrength(bool isOnAir)
    {
        return 0;
    }

    float MoveForceMonitor.getJumpForceStrength(bool isTurble)
    {
        throw new NotImplementedException();
    }

    void MoveForceMonitor.setRigidbodyParamter(Rigidbody rigid)
    {
        throw new NotImplementedException();
    }
}
