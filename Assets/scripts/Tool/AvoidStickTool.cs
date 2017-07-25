using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidStickTool : MonoBehaviour {

    public PlanetMovable pm;
    public bool addForceMaybeStick;
    public float maybeStickValue = -0.35f;
    public float detectForwardOffset = 0.2f;

    private void getGroundNormalPredict(out Vector3 planeNormalPredict)
    {
        Vector3 groundUp = pm.getGroundUp();
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.groundNotBlockCamera;
        RaycastHit hit;
        planeNormalPredict = groundUp;
        Vector3 from = transform.forward * detectForwardOffset + groundUp + transform.position;
        //Debug.DrawRay(from, -groundUp * 2, Color.green);
        if (Physics.Raycast(from, -groundUp, out hit, PlanetMovable.rayCastDistanceToGround, layerMask))
        {
            planeNormalPredict = hit.normal;
            //Debug.DrawRay(hit.point, hit.normal * 5, Color.green);
        }
    }

    public void alongSlopeOrGround(ref Vector3 moveForce, Vector3 planeNormal, Vector3 gravityDir)
    {
        addForceMaybeStick = false;
        //預測斜坡normal
        Vector3 planeNormalPredict;
        getGroundNormalPredict(out planeNormalPredict);

        //要取分量，不然如果玩家不是斜著上坡，算出來的dotValue會改變
        Vector3 N = Vector3.Cross(planeNormal, planeNormalPredict);
        Vector3 partMoveForceAlongSlope =Vector3.ProjectOnPlane(moveForce, N);

        //如果斜坡對PlanetMovalbe存在反作用力夠大的話，就順著斜坡移動
        float dotValue = Vector3.Dot(partMoveForceAlongSlope.normalized, planeNormalPredict);
        if (dotValue < maybeStickValue)
        {
            print("maybeStick");
            moveForce = Vector3.ProjectOnPlane(moveForce, planeNormalPredict);

            //幫忙推一把
            pm.rigid.AddForce(-gravityDir, ForceMode.VelocityChange);
            addForceMaybeStick = true;
        }   
    }
}
