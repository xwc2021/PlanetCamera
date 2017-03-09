using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//類球形星球專用
public class PlanetGravityGenerator : MonoBehaviour, GrounGravityGenerator
{
    public Transform laddingPlanet;
    public Vector3 findGroundUp()
    {
        return (transform.position - laddingPlanet.position).normalized;
    }
}
