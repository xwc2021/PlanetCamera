using UnityEngine;
public interface GroundGravityGenerator
{
    Vector3 findGravityDir(Vector3 headUp, ref Vector3 targetPos);
}
