using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAirCableCarMotor : MonoBehaviour {

    public HingeJoint refHingeJoint;
    Material material;
    public float force = 1500;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        material.color = Color.yellow;
        JointMotor motor = refHingeJoint.motor;
        motor.targetVelocity = force;
        refHingeJoint.motor = motor;
        refHingeJoint.useMotor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        material.color = Color.gray;
    }
}
