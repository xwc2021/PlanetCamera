using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlanetMovable))]
public class FollowerController : MonoBehaviour
{
    PlanetMovable planetMovable;
    public Transform followTarget;
    Rigidbody rigid;
    Animator animator;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        planetMovable.init();
        planetMovable.setTurble(true);
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    Vector3 getMoveForce()
    {
        if (followTarget == null)
            return Vector3.zero;

        Vector3 diff = followTarget.position - followTarget.forward - transform.position; ;
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
        planetMovable.executeMoving(getMoveForce());

        bool moving = rigid.velocity.magnitude > 0.05;
        animator.SetBool("moving", moving);
    }
}
