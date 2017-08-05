using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlanetMovable))]
public class FollowerController : MonoBehaviour, MoveController
{
    PlanetMovable planetMovable;
    public Transform followTarget;
    Rigidbody rigid;
    Animator animator;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
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
        planetMovable.setupGravity();
        planetMovable.setupRequireData();

        planetMovable.executeGravityForce();
        planetMovable.executeMoving();

        bool moving = rigid.velocity.magnitude > 0.05;
        animator.SetBool("moving", moving);
    }
}
