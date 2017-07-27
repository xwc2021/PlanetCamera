using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAirCableCarMotor : MonoBehaviour {

    public HingeJoint hingeJoint;
    Material material;
    public float force = 1500;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        material.color = Color.yellow;
        JointMotor motor = hingeJoint.motor;
        motor.targetVelocity = force;
        hingeJoint.motor = motor;
        hingeJoint.useMotor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        material.color = Color.gray;
    }
}
