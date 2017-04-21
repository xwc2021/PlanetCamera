using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    Vector3 recordPos;
    Vector3 localRight;
	// Use this for initialization
	void Start () {
        recordPos = transform.localPosition;
        localRight=transform.InverseTransformDirection(transform.right);
	}
	
    public float MoveSpin=10f;
    // Update is called once per frame

    public float periodTime=5.0f;
	void FixedUpdate () {

        float factor = 2.0f* Mathf.PI / periodTime;

        float moveLen = MoveSpin * Mathf.Sin(Time.time*factor);
        transform.localPosition = recordPos + localRight * moveLen;
    }
}
