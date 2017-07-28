using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StandingController : MonoBehaviour,MoveController {

    public PlanetMovable planetMovable;

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
