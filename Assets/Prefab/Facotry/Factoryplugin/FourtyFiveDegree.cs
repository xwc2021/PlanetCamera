using UnityEngine;
public class FourtyFiveDegree : MonoBehaviour, FactoryPlugin
{
    public float degree = 45;
    public void doIt(GameObject gameObject)
    {
        // 鎖Camera
        CameraPivot cameraPivot = FindObjectOfType<CameraPivot>();
        Debug.Assert(cameraPivot != null);
        cameraPivot.lockYaw = true;
        cameraPivot.adjustYaw(degree);

        PlanetPlayerController ppc = gameObject.GetComponent<PlanetPlayerController>();
        Debug.Assert(ppc != null);
        ppc.doDergeeLock = true;
    }
}