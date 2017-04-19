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

    public void doScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
