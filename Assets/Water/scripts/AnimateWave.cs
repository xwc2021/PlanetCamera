using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWave : MonoBehaviour {

    public Renderer r;

    float initValue;
    private void Start()
    {
        initValue = r.material.GetFloat("_HS");
    }
    public float period = 5.0f;
    public float newValue;
    // Update is called once per frame
    void Update () {
        newValue = initValue + (1.0f+Mathf.Sin(2.0f*Mathf.PI*(Time.time/ period)))*0.75f;

        r.material.SetFloat("_HS", newValue);
	}
}
