using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddUpForceWhenCollision : MonoBehaviour {

    public PlanetMovable pm;
    void OnCollisionStay(Collision collision)
    {
        //只有layer是Block才作
        bool isBlock = collision.gameObject.layer == 14;
        if (!isBlock)
            return;

        if (!pm.ladding)
        {
            ContactPoint cp = collision.contacts[0];
            Debug.DrawRay(cp.point, 10 * cp.normal, Color.red);

            //screenshot/needUpWhenCollisionSituation.png
            //發現有這2種情況需要addForce，無論那1種都相當於位在圓柱內
            Vector3 groundUp = pm.getGroundUp();
            Vector3 diff = cp.point - transform.position;
            float r = Vector3.ProjectOnPlane(diff, groundUp).magnitude;
            float h = Vector3.Dot(diff, groundUp);
            //print("r="+r+" h="+h);
            //h>1.2表示是因為往上跳撞的
            if (r < 0.6f && h < 1.2f)//比0.65小一點，比1.2低
            {
                print("翻越");
                pm.rigid.AddForce(groundUp * 5, ForceMode.VelocityChange);
            }
        }
    }
}
