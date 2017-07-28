using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingController : MonoBehaviour {

    public PlanetMovable planetMovable;
    private void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.setupRequireData();
        planetMovable.executeGravityForce();
    }
}
