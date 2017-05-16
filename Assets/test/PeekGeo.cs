using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekGeo : MonoBehaviour {

    public Transform r;
    public Transform g;
    public Transform b;
    public void createGS()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        int triCount = mesh.triangles.Length / 3;
        int[] triangles = mesh.triangles;

        Vector4[] tangents = mesh.tangents;
        Vector3[] normals = mesh.normals;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < triCount; i++)
        {

            if (i != 33 && i != 18)
                continue;

            int v0 = triangles[3 * i];
            int v1 = triangles[3 * i + 1];
            int v2 = triangles[3 * i + 2];





            Vector3 center = transform.position + vertices[v0] ;
            Instantiate(r, center, Quaternion.identity, this.transform);

            Vector3 normal = center + normals[v0];
            Instantiate(g, normal, Quaternion.identity, this.transform);

            Vector3 temp = new Vector3(tangents[v0].x, tangents[v0].y, tangents[v0].z);
            Vector3 tangent = center + temp;
            Transform obj=Instantiate(b, tangent, Quaternion.identity, this.transform);
            obj.name = obj.name + "[("+i+")tangent.w=" + tangents[v0].w + "]";

            center = transform.position + vertices[v1];
            Instantiate(r, center, Quaternion.identity, this.transform);

           

            center = transform.position + vertices[v2];
            Instantiate(r, center, Quaternion.identity, this.transform);

           

        }

    }
}
