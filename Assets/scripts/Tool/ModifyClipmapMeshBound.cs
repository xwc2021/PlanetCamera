using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyClipmapMeshBound : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        var bounds = meshFilter.sharedMesh.bounds;
        var nMin = Vector3.Normalize(bounds.min);
        var nMax = Vector3.Normalize(bounds.max);


        var newCenter = (nMax + nMin) / 2;
        meshFilter.sharedMesh.bounds = new Bounds(newCenter, nMax - nMin);
        print("modify");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
