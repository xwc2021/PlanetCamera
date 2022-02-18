using UnityEngine;

//類球形星球專用
public class PlanetGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public Vector3 findGroundUp(ref Vector3 targetPos)
    {
        return (targetPos - transform.position).normalized;
    }
}