using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    public float height = 64.0f;

    void Update()
    {
        var mRenderer = GetComponent<MeshRenderer>();
        var m = mRenderer.material;
        m.SetFloat("_height", this.height);
    }

}
