using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPivot : MonoBehaviour {

    public CameraPivot cameraPivot;
    public bool modifyCameraFollowSpeed = true;
    public void setFollowHighSpeed(bool b)
    {
        if(modifyCameraFollowSpeed)
            cameraPivot.setFollowHighSpeed(b);
    }
}
