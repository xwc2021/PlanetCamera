using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisalbeAirCableMotor : MonoBehaviour {

    public HingeJoint refHingeJoint;
    private void OnTriggerEnter(Collider other)
    {
        refHingeJoint.useMotor = false;
    }
}
