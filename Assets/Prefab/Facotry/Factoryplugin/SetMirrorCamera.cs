using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMirrorCamera : MonoBehaviour,FactoryPlugin {

    public MirrorCamera mirrorCamera;
    public void doIt(GameObject gameObject)
    {
        Camera camera =gameObject.GetComponentInChildren<Camera>();
        Debug.Assert(camera != null);
        mirrorCamera.refCamera = camera;
    }
}
