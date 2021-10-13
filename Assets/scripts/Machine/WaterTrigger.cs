using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{

    [SerializeField]
    WaterParkManager waterParkManager;

    [SerializeField]
    float waterFollowPerSecond = 5;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != TagDefined.Player)
            return;

        int x = (int)this.transform.position.x;
        int z = (int)this.transform.position.z;
        waterParkManager.resetWaterFollowPerSecond(waterFollowPerSecond, x, z);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != TagDefined.Player)
            return;

        int x = (int)this.transform.position.x;
        int z = (int)this.transform.position.z;
        waterParkManager.resetWaterFollowPerSecond(0, x, z);
    }
}
