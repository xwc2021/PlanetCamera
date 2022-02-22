using UnityEngine;
public interface GroundGravityGenerator
{
    Vector3 findGravityDir(Vector3 headUp, Vector3 movablePos, bool isHitFloor, Vector3 hitFloorPos);
}
