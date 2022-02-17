using UnityEngine;

public enum GroundType { Normal, NormalLowGravity, Ice, Seesaw }

public class MoveForceParameterRepository : MonoBehaviour
{
    MoveForceParameter moveForceMonitor;
    public GroundType groundType = GroundType.Normal;
    public MoveForceSetting normal;
    public MoveForceSetting iceSkating;
    public MoveForceSetting seesaw;
    public MoveForceSetting normalLowGravity;//可以用來防止卡斜坡(斜度不能太大)

    public void resetGroundType(GroundType gType, Rigidbody rigid)
    {
        groundType = gType;
        switch (groundType)
        {
            case GroundType.Normal:
                moveForceMonitor = normal as MoveForceParameter;
                break;
            case GroundType.NormalLowGravity:
                moveForceMonitor = normalLowGravity as MoveForceParameter;
                break;
            case GroundType.Ice:
                moveForceMonitor = iceSkating as MoveForceParameter;
                break;
            case GroundType.Seesaw:
                moveForceMonitor = seesaw as MoveForceParameter;
                break;
        }

        moveForceMonitor.setRigidbodyParamter(rigid);
    }

    public MoveForceParameter getMoveForceParameter()
    {
        return moveForceMonitor;
    }
}
