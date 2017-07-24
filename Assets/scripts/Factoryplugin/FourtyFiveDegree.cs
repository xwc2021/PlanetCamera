using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourtyFiveDegree : MonoBehaviour,FactoryPlugin {

    public float degree = 45;
    public void doIt(GameObject gameObject)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, degree, 0);
        CameraPivot cameraPivot = gameObject.GetComponentInChildren<CameraPivot>();
        Debug.Assert(cameraPivot != null);
        cameraPivot.lockYaw = true;

        PlanetPlayerController ppc = gameObject.GetComponent<PlanetPlayerController>();
        Debug.Assert(ppc != null);
        ppc.doDergeeLock = true;
    }
}
