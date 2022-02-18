using UnityEngine;
public class DisalbeAirCableMotor : MonoBehaviour
{
    public HingeJoint refHingeJoint;
    private void OnTriggerEnter(Collider other)
    {
        refHingeJoint.useMotor = false;
    }
}