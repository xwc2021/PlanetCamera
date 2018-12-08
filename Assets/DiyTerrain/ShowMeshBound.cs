using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMeshBound : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        var bounds = meshFilter.mesh.bounds;
        // print(bounds.max);

        Debug.DrawLine(
            transform.TransformPoint(bounds.min),
            transform.TransformPoint(bounds.max),
             Color.yellow);

        // Debug.DrawLine(
        //     transform.TransformPoint(toSphere(bounds.min)),
        //     transform.TransformPoint(toSphere(bounds.max)),
        //      Color.yellow);
    }

    Vector3 toSphere(Vector3 v)
    {
        var localV = v + transform.localPosition;
        var nV = localV.normalized;
        var R = 510.0f;
        return nV * R - transform.localPosition;
    }
}
