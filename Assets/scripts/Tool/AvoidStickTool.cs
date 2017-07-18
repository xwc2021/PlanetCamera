using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidStickTool : MonoBehaviour {

    public PlanetMovable pm;
    public bool addForceOnAir;
    public bool addForceMaybeStick;
    public bool addForcePushWall;
    public float maybeStickValue = -0.35f;
    public float detectForwardOffset = 0.2f;

    //防止在空中卡住
    void OnCollisionStay(Collision collision)
    {
        avoidStickOnAir(collision);
    }

    void avoidStickOnAir(Collision collision)
    {
        addForceOnAir = false;

        if (pm.ladding)
            return;

        //只有layer是Block才作
        bool isBlock = collision.gameObject.layer == LayerDefined.Block;
        if (!isBlock)
            return;

        Vector3 groundUp = pm.getGroundUp();
        ContactPoint cp = collision.contacts[0];

        Vector3 moveForward = transform.forward;
        Vector3 wallNormal = Vector3.ProjectOnPlane(cp.normal, groundUp);

        Vector3 f = Vector3.ProjectOnPlane(moveForward, wallNormal);

        float strengthPerpendicularWall = 0.5f;
        pm.rigid.AddForce(wallNormal * strengthPerpendicularWall, ForceMode.VelocityChange);

        addForceOnAir = true;
    }

    public void resetVariable()
    {
        addForcePushWall = false;
    }

    private void getGroundNormalPredict(out Vector3 planeNormalPredict)
    {
        Vector3 groundUp = pm.getGroundUp();
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.Block;
        RaycastHit hit;
        planeNormalPredict = groundUp;
        Vector3 from = transform.forward * detectForwardOffset + groundUp + transform.position;
        //Debug.DrawRay(from, -groundUp * 2, Color.green);
        if (Physics.Raycast(from, -groundUp, out hit, PlanetMovable.rayCastDistanceToGround, layerMask))
            planeNormalPredict = hit.normal;
    }

    public void alongSlopeOrGround(ref Vector3 moveForce, Vector3 planeNormal, Vector3 gravityDir)
    {
        addForceMaybeStick = false;
        //預測斜坡normal
        Vector3 planeNormalPredict;
        getGroundNormalPredict(out planeNormalPredict);

        //如果斜坡對PlanetMovalbe存在反作用力夠大的話，就順著斜坡移動
        float dotValue = Vector3.Dot(moveForce.normalized, planeNormalPredict);
        if (dotValue < maybeStickValue)
        {
            print("maybeStick");
            moveForce = Vector3.ProjectOnPlane(moveForce, planeNormalPredict);

            //幫忙推一把
            pm.rigid.AddForce(-gravityDir, ForceMode.VelocityChange);
            addForceMaybeStick = true;
        }   
    }

    void getHitWallNormal(out Vector3 wallNormal, out bool isHitWall)
    {
        Vector3 groundUp = pm.getGroundUp();
        int layerMask = 1 << LayerDefined.Block;
        RaycastHit hit;

        isHitWall = false;
        wallNormal = Vector3.zero;

        float SphereR = 0.24f;
        float forwardToWall = 0.5f;
        float leftRightTowall = 0.2f;
        float[] rayCastDistanceToWall = { forwardToWall, forwardToWall, forwardToWall, leftRightTowall, leftRightTowall };


        float heightThreshold = 0.1f;
        float[] height = { 0.25f, 0.6f, 1.2f, 0.6f, 0.6f };
        Vector3[] dir = { transform.forward, transform.forward, transform.forward, transform.right, -transform.right };
        for (int i = 0; i < 5; i++)
        {
            Vector3 from = groundUp * height[i] + transform.position;
            Debug.DrawRay(from, dir[i] * rayCastDistanceToWall[i], Color.green);
            if (Physics.SphereCast(from, SphereR, dir[i], out hit, rayCastDistanceToWall[i], layerMask))
            {
                float h = Vector3.Dot(hit.point - transform.position, groundUp);

                //高過才算
                if (h > heightThreshold)
                {
                    isHitWall = true;
                    wallNormal = hit.normal;
                    Debug.DrawRay(hit.point, hit.normal * 2, Color.red);
                    Debug.DrawRay(from, transform.forward, Color.yellow);
                    return;
                }
            }
        }
    }

    public void setNewMoveForceAlongWall( ref Vector3 moveForce)
    {
        bool isHit;
        Vector3 wallNormal;
        getHitWallNormal(out wallNormal, out isHit);

        addForcePushWall = false;
        if (!isHit)
            return;

        Vector3 groundUp = pm.getGroundUp();
        bool onAir = !pm.ladding;

        wallNormal = Vector3.ProjectOnPlane(wallNormal, groundUp);
        Debug.DrawRay(transform.position, wallNormal, Color.red);
        float dotValue = Vector3.Dot(moveForce, wallNormal);

        Vector3 newMoveForce = Vector3.ProjectOnPlane(moveForce, wallNormal);
        //如果移動的方向和wallNormal接近垂直，newMoveForce就可能變的很短
        bool isSetNewValueAlongWall = newMoveForce.magnitude > 0.01f;
        if (isSetNewValueAlongWall)
        {
            moveForce = newMoveForce;

            float strength = 0.5f;
            pm.rigid.AddForce(wallNormal * strength, ForceMode.VelocityChange);
            addForcePushWall = true;
        }
    }
}
