using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class StandingController : MonoBehaviour
{
    PlanetMovable planetMovable;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        planetMovable.init();
    }

    private void FixedUpdate()
    {
        planetMovable.preProcess(false, true);
        planetMovable.executeGravityForce();
    }
}