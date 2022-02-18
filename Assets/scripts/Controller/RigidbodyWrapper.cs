using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class RigidbodyWrapper : MonoBehaviour
{
    PlanetMovable planetMovable;
    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
    }

    private void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.executeGravityForce();
    }
}