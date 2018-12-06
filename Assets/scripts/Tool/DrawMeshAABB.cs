using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMeshAABB : MonoBehaviour
{

    public Transform box;
    MeshFilter meshFilter;
    // Use this for initialization
    void Start()
    {
        meshFilter = this.GetComponent<MeshFilter>();

    }

    // Update is called once per frame
    void Update()
    {
        if (meshFilter != null)
        {
            Bounds b = meshFilter.sharedMesh.bounds;
            box.transform.position = transform.TransformPoint(b.center);
            box.transform.localScale = 2 * b.extents;
        }
    }
}
