using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRigidBody : MonoBehaviour {

    public PlanetMovable pm;
	// Use this for initialization
	void Start () {
        transform.parent = null;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = pm.transform.position;
        transform.rotation = pm.transform.rotation;
	}

    void OnCollisionStay(Collision collision)
    {
        //翻越
        upWhenCollision(collision);
    }

    void upWhenCollision(Collision collision)
    {
        //只有layer是Block才作
        bool isBlock = collision.gameObject.layer == 14;
        if (!isBlock)
            return;

        if (!pm.ladding)
        {
            print("翻越");
            ContactPoint cp = collision.contacts[0];
            Debug.DrawRay(cp.point, 10 * cp.normal, Color.red);

            pm.rigid.AddForce(pm.getGroundUp() * 5, ForceMode.VelocityChange);
        }
    }
}
