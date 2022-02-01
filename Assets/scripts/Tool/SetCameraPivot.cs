using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPivot : MonoBehaviour
{
    public CameraPivot cameraPivot;
    public void setFollowSpeed(bool b)
    {
        cameraPivot.setFollowSpeed(b);
    }
}
