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
        planetMovable.setupGravityDir();
        planetMovable.setupContactDataAndHeadUp();
        planetMovable.executeGravityForce();
    }
}