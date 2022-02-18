using UnityEngine;
public class SetParent : MonoBehaviour
{

    [SerializeField]
    bool cameraFollowUsingHighSpeed = false;

    void OnTriggerStay(Collider other)
    {
        if (!TagDefined.canOnMovableSet(other.gameObject.tag))
            return;

        other.transform.parent = transform;

        if (cameraFollowUsingHighSpeed)
        {
            SetCameraPivot setCameraPivot = other.gameObject.GetComponent<SetCameraPivot>();
            if (setCameraPivot != null)
                setCameraPivot.setFollowSpeed(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!TagDefined.canOnMovableSet(other.gameObject.tag))
            return;

        other.transform.parent = null;

        if (cameraFollowUsingHighSpeed)
        {
            SetCameraPivot setCameraPivot = other.gameObject.GetComponent<SetCameraPivot>();
            if (setCameraPivot != null)
                setCameraPivot.setFollowSpeed(false);
        }
    }
}