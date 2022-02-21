using UnityEngine;
public interface GroundGravityGenerator
{
    Vector3 findGravityDir(Vector3 headUp, Vector3 targetPos, bool isHitFloor, Vector3 hitFloorPos);
}
