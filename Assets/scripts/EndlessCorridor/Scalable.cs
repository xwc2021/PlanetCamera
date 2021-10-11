using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalable : MonoBehaviour
{

    public CameraPivot cameraPivot;
    public void doScale(float scale)
    {
        //鎖scale的增加值(為了減少縮放player時，camera的抖動)
        //原來的scale會浮動
        scale = scale - (scale % 0.1f);

        //改成呼叫setCameraFollowHighSpeed
        transform.localScale = new Vector3(scale, scale, scale);

        cameraPivot.resetTargetRScale(scale);
    }

    public void resetScale()
    {
        transform.localScale = Vector3.one;
        cameraPivot.resetRScale();
    }

    public void resetPos(float scaleValue)
    {
        cameraPivot.resetRecordPos(this.transform.position, scaleValue);
        transform.position = Vector3.zero;
    }
}
