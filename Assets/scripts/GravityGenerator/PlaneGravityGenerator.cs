using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//平面
public class PlaneGravityGenerator : MonoBehaviour, GrounGravityGenerator
{
    public Vector3 findGroundUp()
    {
        return Vector3.up;
    }
}
