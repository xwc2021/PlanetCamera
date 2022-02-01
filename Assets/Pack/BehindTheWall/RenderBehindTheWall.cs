using UnityEngine;

public class RenderBehindTheWall : MonoBehaviour
{
    MeshRenderer[] meshRenderers;
    MeshFilter[] meshFilters;
    SkinnedMeshRenderer[] skinnedMeshRenderers;
    Mesh[] bakeMeshs;

    void Awake()
    {
        //會往下層節點尋找
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshFilters = GetComponentsInChildren<MeshFilter>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        bakeMeshs = new Mesh[skinnedMeshRenderers.Length];
        for (var i = 0; i < bakeMeshs.Length; i++)
            bakeMeshs[i] = new Mesh();
    }

    void Start()
    {
        AjustRenderQueue((int)RenderBehindTheWallCommandBuffer.getInstance().GetQueueOrderForMainBody());
    }

    //動態修改renderQueue
    void AjustRenderQueue(int renderQueueOrder)
    {
        foreach (var mr in meshRenderers)
        {
            foreach (var m in mr.materials)
            {
                m.renderQueue = renderQueueOrder;
            }
        }

        foreach (var smr in skinnedMeshRenderers)
        {
            foreach (var m in smr.materials)
            {
                m.renderQueue = renderQueueOrder;
            }
        }
    }

    void DrawAll()
    {
        foreach (var mf in meshFilters)
            DrawMesh(mf);

        for (var i = 0; i < skinnedMeshRenderers.Length; i++)
            DrawSkin(skinnedMeshRenderers[i], bakeMeshs[i]);
    }

    void BackSkinMesh(SkinnedMeshRenderer skinnedMeshRenderer, Mesh mesh)
    {
        skinnedMeshRenderer.BakeMesh(mesh);
    }

    void DrawSkin(SkinnedMeshRenderer skinnedMeshRenderer, Mesh mesh)
    {
        //因為BakeMesh後模型會包含縮放的結果
        //而localToWorldMatrix也會包含縮放
        //如果直接拿來使用
        //(比如說x方向scale=2，結果會是scale=4)
        //(比如說x方向scale=1/2，結果會是scale=1/4)
        var matrix = skinnedMeshRenderer.localToWorldMatrix;

        //(因為角色會演出動作
        //所以skinnedMeshRenderer.localToWorldMatrix和parent.transform.localToWorldMatrix會有出入)
        //改成下面的方法：取出3軸向量正規化
        Vector3 x = matrix.GetColumn(0);
        Vector3 y = matrix.GetColumn(1);
        Vector3 z = matrix.GetColumn(2);

        Vector4 nX = x.normalized; nX.w = 0;
        Vector4 nY = y.normalized; nY.w = 0;
        Vector4 nZ = z.normalized; nZ.w = 0;

        matrix.SetColumn(0, nX);
        matrix.SetColumn(1, nY);
        matrix.SetColumn(2, nZ);

        RenderBehindTheWallCommandBuffer.getInstance().DrawBehindTheWall(mesh, ref matrix);
    }

    void DrawMesh(MeshFilter meshFilter)
    {
        var mesh = meshFilter.mesh;
        var matrix = meshFilter.transform.localToWorldMatrix;
        RenderBehindTheWallCommandBuffer.getInstance().DrawBehindTheWall(mesh, ref matrix);
    }

    void LateUpdate()
    {
        //只需要BackSkinMesh一次
        for (var i = 0; i < skinnedMeshRenderers.Length; i++)
            BackSkinMesh(skinnedMeshRenderers[i], bakeMeshs[i]);

        DrawAll();
    }
}
