using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMeshBound : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        var bounds = meshFilter.sharedMesh.bounds;
        print(bounds.max);

        Debug.DrawLine(
            transform.TransformPoint(bounds.min),
            transform.TransformPoint(bounds.max),
             Color.yellow);
    }
}
