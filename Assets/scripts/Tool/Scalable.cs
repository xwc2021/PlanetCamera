using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public CameraPivot camera;
    public void doScale(float scale,bool snap = true,bool lerpCameraR=true)
    {
        //鎖scale的增加值(為了減少縮放player時，camera的抖動)
        //原來的scale會浮動
        if(snap)
            scale = scale - (scale % 0.1f);
        transform.localScale = new Vector3(scale, scale, scale);

        if(lerpCameraR)
            camera.resetTargetRScale(scale);
        else
            camera.resetRScale(scale);
    }
}
