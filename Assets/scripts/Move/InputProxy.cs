using UnityEngine;

public interface InputProxy
{
    Vector2 getHV();
    bool pressJump();
    bool pressFire();
    bool holdFire();
    float yawScale();
    float pitchScale();
    bool enableControlUI();
}
