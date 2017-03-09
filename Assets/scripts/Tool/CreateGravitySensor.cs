using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGravitySensor : MonoBehaviour {

    public Transform gs;

    //GravitySensor的作用：GravisySensor的axix forward會指出地面(平面)的方向
    //使用時記得產生後再旋轉物件，不然位置會對不到
    //再改成旋轉後可以使用的話，要再加一層GameObject，然後把動態產生的GS都放到它底下，之後旋轉GameObject就行了，以後再說
    public void createGS()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        int triCount =mesh.triangles.Length / 3;
        int [] triangles = mesh.triangles;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < triCount; i++)
        {
            int v0 = triangles[3 * i];
            int v1 = triangles[3 * i +1];
            int v2 = triangles[3 * i +2];

            Vector3 vector01 = vertices[v1] - vertices[v0];
            Vector3 vector02 = vertices[v2] - vertices[v0];

            //方向錯的話，作外積時交換vector01和vector02就行了
            //不過就結果來說，三角形應該是順時針order
            Vector3 Normal =Vector3.Cross(vector01, vector02);
            Quaternion rot =Quaternion.LookRotation(Normal);

            Vector3 center = transform.position+(vertices[v0]+vertices[v1] + vertices[v2])/3;

            Instantiate(gs,center,rot, this.transform);
        }
        
    }
}
