using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkatingToggle : MonoBehaviour {

    

    private void OnTriggerEnter(Collider other)
    {
        PlanetMovable pm = other.GetComponent<PlanetMovable>();
        pm.enableIceSkatingRigid();
    }

    private void OnTriggerExit(Collider other)
    {
        PlanetMovable pm = other.GetComponent<PlanetMovable>();
        pm.enableNormalRigid();
    }
}
