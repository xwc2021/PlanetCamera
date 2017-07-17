using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    Vector3 recordPos;
	// Use this for initialization
	void Start () {
        recordPos = transform.position;
    }
	
    public float MoveSpin=10f;
    // Update is called once per frame

    public float periodTime=5.0f;
	void FixedUpdate () {

        float factor = 2.0f* Mathf.PI / periodTime;

        float moveLen = MoveSpin * Mathf.Sin(Time.time*factor);

        transform.position = Vector3.Lerp(transform.position, recordPos + transform.right * moveLen,Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        other.transform.parent = transform;
 
        SetCameraPivot setCameraPivot= other.gameObject.GetComponent<SetCameraPivot>();
        if (setCameraPivot != null)
           setCameraPivot.setFollowHighSpeed(true);
    }

    void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;

        SetCameraPivot setCameraPivot = other.gameObject.GetComponent<SetCameraPivot>();
        if (setCameraPivot != null)
           setCameraPivot.setFollowHighSpeed(false);
    }
}
