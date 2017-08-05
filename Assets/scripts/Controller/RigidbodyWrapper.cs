using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class RigidbodyWrapper : MonoBehaviour, MoveController
{

    PlanetMovable planetMovable;
    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
    }

    private void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.executeGravityForce();
    }

    bool MoveController.doTurbo()
    {
        return false;
    }

    Vector3 MoveController.getMoveForce()
    {
        return Vector3.zero;
    }
}
