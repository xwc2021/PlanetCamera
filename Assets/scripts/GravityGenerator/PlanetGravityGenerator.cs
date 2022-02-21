using UnityEngine;

//類球形星球專用
public class PlanetGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public Vector3 findGravityDir(Vector3 headUp, Vector3 targetPos, bool isHitFloor, Vector3 hitFloorPos)
    {
        return (transform.position - targetPos).normalized;
    }
}