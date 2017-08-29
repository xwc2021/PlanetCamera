using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour {

    [SerializeField]
    WaterParkManager waterParkManager;

    [SerializeField]
    float waterFollowPerSecond = 5;

    private void OnTriggerEnter(Collider other)
    {
        waterParkManager.resetWaterFollowPerSecond(waterFollowPerSecond);
    }

    private void OnTriggerExit(Collider other)
    {
        waterParkManager.resetWaterFollowPerSecond(0);
    }
}
