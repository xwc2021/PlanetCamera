using UnityEngine;
public interface GroundGravityGenerator
{
    Vector3 findGroundUp(ref Vector3 targetPos);
}
