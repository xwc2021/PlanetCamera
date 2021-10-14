using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByAxis : MonoBehaviour {

    public Transform target;
	// Use this for initialization
	void Start () {
        Vector3 targetNormal = target.up;
        Vector3 nowNormal = transform.up;
        Vector3 axis =Vector3.Cross(nowNormal,targetNormal);
        float dotValue =Vector3.Dot(nowNormal,targetNormal);
        float degree =Mathf.Rad2Deg*Mathf.Acos(dotValue);
        print(degree);

        Quaternion rot = Quaternion.AngleAxis(degree,axis);
        transform.rotation = rot * transform.rotation;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
