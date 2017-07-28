using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class FollowerController : MonoBehaviour, MoveController
{
    public PlanetMovable planetMovable;
    public Transform followTarget;

    bool MoveController.doJump()
    {
        return false;
    }

    bool MoveController.doTurbo()
    {
        return true;
    }

    Vector3 MoveController.getMoveForce()
    {
        if (followTarget == null)
            return Vector3.zero;

        Vector3 diff = followTarget.position - transform.position; ;
        if (diff.magnitude < 0.5)
            return Vector3.zero;

        Vector3 controllForce = diff;

        controllForce = Vector3.ProjectOnPlane(controllForce, transform.up);

        controllForce.Normalize();

        return controllForce;
    }

    private void FixedUpdate()
    {
        planetMovable.gravitySetup();
        planetMovable.dataSetup();

        planetMovable.processGravity();
        planetMovable.processMoving();
    }
}
