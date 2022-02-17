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

        waterParkManager.resetWaterFollowPerSecond(waterFollowPerSecond);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != TagDefined.Player)
            return;

        waterParkManager.resetWaterFollowPerSecond(0);
    }
}