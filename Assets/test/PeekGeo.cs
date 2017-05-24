using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekGeo : MonoBehaviour {

    public NormalMapAxis axis;


    void createAxis(int index)
    {
        Vector3 original = transform.position + vertices[index];

        NormalMapAxis obj = Instantiate<NormalMapAxis>(axis, original, Quaternion.identity);
        obj.initByData(tangents[index], normals[index]);
        obj.transform.parent = this.transform;
    }

    Mesh mesh;
    int triCount;
    int[] triangles;
    Vector4[] tangents;
    Vector3[] normals;
    Vector3[] vertices;

    public void clearAll()
    {
        int childCount =transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    public void createTri3Axis(int triIndex)
    {
        Quaternion tempRot= transform.rotation;
        //強制取消旋轉
        transform.rotation = Quaternion.identity;

        if (mesh == null)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;

            triCount = mesh.triangles.Length / 3;
            triangles = mesh.triangles;

            tangents = mesh.tangents;
            normals = mesh.normals;
            vertices = mesh.vertices;
        }

        print("tangents length="+tangents.Length);
        if (tangents.Length == 0)
        {
            print("沒有tangents資料");
            return;
        }

        if(triIndex> triCount)
        {
            print("三角形索引超出範圍");
            return;
        }

        int v0 = triangles[3 * triIndex];
        int v1 = triangles[3 * triIndex + 1];
        int v2 = triangles[3 * triIndex + 2];

        createAxis(v0);
        createAxis(v1);
        createAxis(v2);

        //復原
        transform.rotation = tempRot;
    }
}
