using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAABB : MonoBehaviour {

    public Transform box;
    Collider mCollider;
	// Use this for initialization
	void Start () {
        mCollider = GetComponent<Collider>();

    }
	
	// Update is called once per frame
	void Update () {
        if (mCollider != null)
        {
            Bounds b = mCollider.bounds;
            box.transform.position = b.center;
            box.transform.localScale = 2*b.extents;
        }
	}
}
