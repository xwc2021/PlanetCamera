using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResetShereTerrain : MonoBehaviour
{

    void resetLocalPos()
    {
        var sTerrain = this.GetComponent<SphereTerrain>();
        sTerrain.updateLocalPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            // code executed in edit mode
            this.resetLocalPos();
        }
    }
}
