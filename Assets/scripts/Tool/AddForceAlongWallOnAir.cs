using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceAlongWallOnAir : MonoBehaviour {

    public PlanetMovable pm;
    public bool addForce;
    //防止在空中卡住
    void OnCollisionStay(Collision collision)
    {
        addForce = false;

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

        addForce = true;
    }
}
