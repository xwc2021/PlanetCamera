using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourtyFiveDegree : MonoBehaviour,FactoryPlugin {

    public void doIt(GameObject gameObject)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 45, 0);
        CameraPivot cameraPivot = gameObject.GetComponentInChildren<CameraPivot>();

        Debug.Assert(cameraPivot != null);
        cameraPivot.lockYaw = true;
    }
}
