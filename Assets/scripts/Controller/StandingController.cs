using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class StandingController : MonoBehaviour, MoveController
{

    PlanetMovable planetMovable;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        planetMovable.init(this);
    }

    bool MoveController.doTurbo()
    {
        return false;
    }

    private void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.setupRequireData();
        planetMovable.executeGravityForce();
    }

    Vector3 MoveController.getMoveForce()
    {
        return Vector3.zero;
    }
}
