using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisalbeAirCableMotor : MonoBehaviour {

    public HingeJoint hingeJoint;
    private void OnTriggerEnter(Collider other)
    {
        hingeJoint.useMotor = false;
    }
}
