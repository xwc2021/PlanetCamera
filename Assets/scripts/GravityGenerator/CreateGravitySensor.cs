using UnityEngine;
public class CreateGravitySensor : MonoBehaviour
{
    public Transform gs;

    // GravitySensor的作用：GravisySensor的axix forward會指出地面(平面)的方向
    // 使用時記得產生後再旋轉物件，不然位置會對不到
    public void createGS()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        var modelPos = transform.position;
        int vCount = mesh.vertices.Length;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < vCount; i++)
        {
            var v0 = vertices[i];
            var n0 = normals[i];
            Quaternion rot0 = Quaternion.LookRotation(n0);
            Instantiate(gs, v0 + modelPos, rot0, this.transform);
        }
    }
}