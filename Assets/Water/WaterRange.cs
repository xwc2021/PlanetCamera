using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRange : MonoBehaviour {

    public Transform borderDummy;
    // Use this for initialization

    public Renderer r;
    private void Start()
    {
        r.material.SetFloat("_W", getWidth());
        r.material.SetFloat("_H", getHeight());
    }

    public float getWidth() { return borderDummy.localPosition.x; }
    public float getHeight() { return borderDummy.localPosition.y; }
}
