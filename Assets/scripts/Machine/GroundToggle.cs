using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundToggle : MonoBehaviour {

    public enum GroundType {Ice,Seeswa }

    public GroundType groundType = GroundType.Ice;

    private void OnTriggerEnter(Collider other)
    {
        PlanetMovable pm = other.GetComponent<PlanetMovable>();
        if (pm == null)
            return;

        switch (groundType)
        {
            case GroundType.Ice:
                pm.enableIceSkating();
                break;

            case GroundType.Seeswa:
                pm.enableSeesaw();
                break;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        PlanetMovable pm = other.GetComponent<PlanetMovable>();
        if (pm == null)
            return;

        pm.enableNormal();
    }
}
