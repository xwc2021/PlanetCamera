using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class StandingController : MonoBehaviour
{

    PlanetMovable planetMovable;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        planetMovable.init();
    }

    private void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.setupRequireData();
        planetMovable.executeGravityForce();
    }
}
