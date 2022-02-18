using UnityEngine;
public interface MoveForceParameter
{
    float getMoveForceStrength(bool isOnAir, bool isTurble);
    float getGravityForceStrength(bool isOnAir);
    float getJumpForceStrength(bool isTurble);
    void setRigidbodyParamter(Rigidbody rigid);
}
