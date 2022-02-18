using UnityEngine;
public class GroundTypeSetter : MonoBehaviour
{
    public GroundType groundType = GroundType.Ice;

    private void OnTriggerEnter(Collider other)
    {
        PlanetMovable pm = other.GetComponent<PlanetMovable>();
        if (pm == null)
            return;

        pm.resetGroundType(groundType);
    }

    private void OnTriggerExit(Collider other)
    {
        PlanetMovable pm = other.GetComponent<PlanetMovable>();
        if (pm == null)
            return;

        pm.resetGroundType(GroundType.Normal);
    }
}