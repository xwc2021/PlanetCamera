using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScalable : MonoBehaviour,FactoryPlugin {

    public EndlessCorridorManager ecManager;
    public void doIt(GameObject gameObject)
    {
        Scalable scalable =gameObject.AddComponent<Scalable>();
        CameraPivot cameraPivot = gameObject.GetComponentInChildren<CameraPivot>();

        Debug.Assert(cameraPivot != null);
        Debug.Assert(ecManager != null);

        ecManager.player = scalable;
        scalable.cameraPivot = cameraPivot;
        ecManager.flyCamara = cameraPivot;
    }
}
