using UnityEngine;
public class SetCameraPivot : MonoBehaviour
{
    public CameraPivot cameraPivot;
    public void setFollowSpeed(bool b)
    {
        cameraPivot.setFollowSpeed(b);
    }
}