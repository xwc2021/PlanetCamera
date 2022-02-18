using UnityEngine;

//類球形星球專用
public class PlanetGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public Vector3 findGravityDir(Vector3 headUp, ref Vector3 targetPos)
    {
        return (transform.position - targetPos).normalized;
    }
}