using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderBehindTheWall : MonoBehaviour {

    MeshRenderer[] meshRenderers;
    MeshFilter[] meshFilters;
    SkinnedMeshRenderer[] skinnedMeshRenderers;
    Mesh[] bakeMeshs;

    //這樣才能抽換
    delegate void DrawMeshFun(Mesh mesh, ref Matrix4x4 matrix);
    DrawMeshFun mDrawBehindTheWallFun;
    DrawMeshFun mDrawMaskFun;

    void Awake()
    {
        //會往下層節點尋找
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshFilters = GetComponentsInChildren<MeshFilter>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        bakeMeshs = new Mesh[skinnedMeshRenderers.Length];
        for (var i=0;i< bakeMeshs.Length;i++)
            bakeMeshs[i] = new Mesh();

        mDrawBehindTheWallFun = RenderBehindTheWallCommandBuffer.getInstance().DrawBehindTheWall;
        mDrawMaskFun = RenderBehindTheWallCommandBuffer.getInstance().DrawMask;   
    }

    RenderBehindTheWallCamera.QueueOrderForMainBody queueOrderForMainBody;
    void Start()
    {
        queueOrderForMainBody = RenderBehindTheWallCommandBuffer.getInstance().GetQueueOrderForMainBody();
        AjustRenderQueue((int)queueOrderForMainBody);
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

    void DrawAll(DrawMeshFun fun)
    {
        foreach (var mf in meshFilters)
            DrawMesh(mf,fun);

        for (var i = 0; i < skinnedMeshRenderers.Length; i++)
            DrawSkin(skinnedMeshRenderers[i], bakeMeshs[i], fun);
    }

    void BackSkinMesh(SkinnedMeshRenderer skinnedMeshRenderer,Mesh mesh)
    {
        skinnedMeshRenderer.BakeMesh(mesh);
    }

    void DrawSkin(SkinnedMeshRenderer skinnedMeshRenderer,Mesh mesh,DrawMeshFun fun)
    {
        var matrix = skinnedMeshRenderer.transform.localToWorldMatrix;
        fun(mesh, ref matrix);
    }

    void DrawMesh(MeshFilter meshFilter, DrawMeshFun fun) {
        var mesh = meshFilter.mesh;
        var matrix = meshFilter.transform.localToWorldMatrix;
        fun(mesh, ref matrix);
    }

    void LateUpdate()
    {
        //只需要BackSkinMesh一次
        for (var i = 0; i < skinnedMeshRenderers.Length; i++)
            BackSkinMesh(skinnedMeshRenderers[i], bakeMeshs[i]);

        if (queueOrderForMainBody == RenderBehindTheWallCamera.QueueOrderForMainBody.MainBodyAfterMask)
            DrawAll(mDrawMaskFun);

        DrawAll(mDrawBehindTheWallFun);
    }




}
