using UnityEngine;
public interface GroundGravityGenerator
{
    Vector3 findGroundUp(Vector3 headUp, ref Vector3 targetPos);
}
