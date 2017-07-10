using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceAlongWall : MonoBehaviour {

    public PlanetMovable pm;
    void OnCollisionStay(Collision collision)
    {
        //況著牆移動
       
        //在空中不作
        if (!pm.ladding)
            return;

        //只有layer是Block才作
        bool isBlock = collision.gameObject.layer == 14;
        if (!isBlock)
            return;

        Vector3 groundUp = pm.getGroundUp();
        ContactPoint cp = collision.contacts[0];

        //cp.point在腳邊不作
        float dotValue = Vector3.Dot(cp.point - transform.position, groundUp);
        if (dotValue < 0.1)
            return;
        Debug.DrawRay(transform.position, 10 * cp.normal, Color.red);

        Vector3 moveForward = transform.forward;
        Vector3 wallNormal = Vector3.ProjectOnPlane(cp.normal, groundUp);

        Debug.DrawRay(transform.position, wallNormal, Color.yellow);
        Vector3 f = Vector3.ProjectOnPlane(moveForward, wallNormal);

        //當moveForward和f接近平行時k值要小
        //當moveForward和f接近垂直時k值要大
        float dot = Vector3.Dot(moveForward, f);
        float k = 1 - dot;
        k = Mathf.Max(k, 0.2f);

        float strength = 2.5f;
        pm.rigid.AddForce(f * k * strength, ForceMode.VelocityChange);
        //rigid.AddForce(f * k * strength, ForceMode.Acceleration);

        float strengthPerpendicularWall = 1;

        pm.rigid.AddForce(wallNormal * strengthPerpendicularWall, ForceMode.VelocityChange);
        Debug.DrawRay(transform.position, 10 * f, Color.blue);
    }
}
