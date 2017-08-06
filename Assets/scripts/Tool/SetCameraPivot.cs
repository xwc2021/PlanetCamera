using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPivot : MonoBehaviour {

    public CameraPivot cameraPivot;
    public void setFollowHighSpeed(bool b)
    {
        cameraPivot.setFollowHighSpeed(b);
    }
}
