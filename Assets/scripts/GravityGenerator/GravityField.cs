using UnityEngine;
public class GravityField : MonoBehaviour
{
    public MonoBehaviour groundGravityGenerator;
    GroundGravityGenerator iGroundGravityGenerator;

    public void Awake()
    {
        iGroundGravityGenerator = groundGravityGenerator as GroundGravityGenerator;
    }

    private void OnTriggerEnter(Collider collider)
    {
        var planetMovable = collider.GetComponent<PlanetMovable>();
        if (planetMovable == null)
            return;

        planetMovable.ResetGravityGenetrator(iGroundGravityGenerator);
    }
}