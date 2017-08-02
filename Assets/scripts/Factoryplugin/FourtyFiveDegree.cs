using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourtyFiveDegree : MonoBehaviour,FactoryPlugin {

    public float degree = 45;
    public void doIt(GameObject gameObject)
    {
        CameraPivot cameraPivot = gameObject.GetComponentInChildren<CameraPivot>();
        Debug.Assert(cameraPivot != null);
        cameraPivot.lockYaw = true;
        cameraPivot.adjustYaw(degree);

        PlanetPlayerController ppc = gameObject.GetComponent<PlanetPlayerController>();
        Debug.Assert(ppc != null);
        ppc.doDergeeLock = true;
    }
}
