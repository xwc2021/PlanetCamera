using UnityEngine;
public class SetMirrorCamera : MonoBehaviour, FactoryPlugin
{

    public MirrorCamera mirrorCamera;
    public void doIt(GameObject gameObject)
    {
        Camera camera = FindObjectOfType<CameraPivot>().getCamera();
        Debug.Assert(camera != null);
        mirrorCamera.refCamera = camera;
    }
}