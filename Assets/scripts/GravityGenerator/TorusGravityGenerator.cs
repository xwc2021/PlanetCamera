using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public Vector3 findGroundUp(ref Vector3 targetPos)
    {
        return Vector3.zero;
    }
}
