using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRange : MonoBehaviour {

    public Transform diffDummy;
    public Transform borderDummy;
    // Use this for initialization

    public Renderer []r;
    private void Start()
    {
        foreach (Renderer ele in r)
        {
            ele.material.SetFloat("_W", getWidth());
            ele.material.SetFloat("_H", getHeight());

            ele.material.SetFloat("_dW", getDiffWidth());
            ele.material.SetFloat("_dH", getDiffHeight());
        }
        
    }

    public float getWidth() { return borderDummy.localPosition.x; }
    public float getHeight() { return borderDummy.localPosition.y; }

    public float getDiffWidth() { return diffDummy.localPosition.x; }
    public float getDiffHeight() { return diffDummy.localPosition.y; }
}
