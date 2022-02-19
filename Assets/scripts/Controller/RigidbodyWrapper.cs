using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class RigidbodyWrapper : MonoBehaviour
{
    PlanetMovable planetMovable;
    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        planetMovable.init();
    }

    private void FixedUpdate()
    {
        planetMovable.preProcess(false, false);
        planetMovable.executeGravityForce();
    }
}