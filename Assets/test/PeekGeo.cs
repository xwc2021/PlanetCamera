using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekGeo : MonoBehaviour {

    public NormalMapAxis axis;


    void createAxis(int index,float scale)
    {
        NormalMapAxis obj = Instantiate<NormalMapAxis>(axis, Vector3.zero, Quaternion.identity);
        obj.name += "[" + triIndex + "]";

        //旋轉沒差，因為parent的世界旋轉已經設成Quaternion.identity
        obj.initByData(tangents[index], normals[index]);

        obj.transform.parent = this.transform;

        //這裡先parent再scale
        //如果是先scale再parent，parent(或是其上層transfrom)已經如果有縮放過
        //obj的scale值會再被重新校正，以符合parent前在世界上的縮放值
        obj.transform.localScale = new Vector3(scale, scale, scale);

        //position也有上面一樣的面題
        obj.transform.localPosition = vertices[index];


    }

    Mesh mesh;
    int triCount;
    int[] triangles;
    Vector4[] tangents;
    Vector3[] normals;
    Vector3[] vertices;

    public int triIndex=0;
    public float AxisScale=1.0f;

    public void clearAll()
    {
        int childCount =transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    public void createTri3Axis()
    {
        Quaternion tempRot= transform.rotation;
        Vector3 tempScale = transform.localScale;

        //強制取消旋轉
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        if (mesh == null)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;

            triCount = mesh.triangles.Length / 3;
            triangles = mesh.triangles;

            tangents = mesh.tangents;
            normals = mesh.normals;
            vertices = mesh.vertices;
        }

        
        if (tangents.Length == 0)
        {
            print("沒有tangents資料");
            return;
        }

        print("triCount =" + triCount);
        if (triIndex>= triCount)
        {
            print("三角形索引超出範圍");
            return;
        }

        int v0 = triangles[3 * triIndex];
        int v1 = triangles[3 * triIndex + 1];
        int v2 = triangles[3 * triIndex + 2];

        createAxis(v0, AxisScale);
        createAxis(v1, AxisScale);
        createAxis(v2, AxisScale);

        //復原
        transform.rotation = tempRot;
        transform.localScale = tempScale;
    }
}
