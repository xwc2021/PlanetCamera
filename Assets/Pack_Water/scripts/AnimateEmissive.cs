using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEmissive : MonoBehaviour {

    public Renderer r;
    Color color;
    // Use this for initialization
    void Start () {
        color =r.material.GetColor("_Color");

    }
	
	// Update is called once per frame
	void Update () {
        float emission = Mathf.PingPong(5.0f*Time.time,1.5f);
        r.material.SetColor("_EmissionColor", color * emission);
    }
}
