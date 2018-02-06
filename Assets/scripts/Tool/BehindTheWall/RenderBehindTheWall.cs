using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//OnWillRenderObject需要加在有MeshFilter或SkinnedMeshRenderer元件的GameObject身上才會觸發!!
public class RenderBehindTheWall : MonoBehaviour {

    MeshRenderer[] meshRenderers;
    MeshFilter[] meshFilters;
    SkinnedMeshRenderer[] skinnedMeshRenderers;

    //這樣才能抽換
    delegate void DrawMeshFun(Mesh mesh, ref Matrix4x4 matrix);
    DrawMeshFun mDrawCommandBufferFun;
    DrawMeshFun mDrawFun;

    void Awake()
    {
        //會往下層節點尋找
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshFilters = GetComponentsInChildren<MeshFilter>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        mDrawCommandBufferFun = RenderBehindTheWallCommandBuffer.getInstance().DrawCommandBuffer;
        mDrawFun = RenderBehindTheWallCommandBuffer.getInstance().Draw;

        AjustRenderQueue();
    }

    void AjustRenderQueue()
    {
        int Geometry = 2000;
        int offset = 50;
        foreach (var mr in meshRenderers)
        {
            foreach (var m in mr.materials)
            {
                print(m.renderQueue);
                m.renderQueue = Geometry + offset;
                print("new ="+m.renderQueue);
            }
        }

        foreach (var smr in skinnedMeshRenderers)
        {
            foreach (var m in smr.materials)
            {
                m.renderQueue = Geometry + offset;
            }
        }
    }

    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    //OnBecameVisible只有第1個camera看到物件才會觸發
    //OnWillRenderObject需要加在有MeshFilter或SkinnedMeshRenderer元件的GameObject身上才會觸發!!
    void OnWillRenderObject()
    {
        if (!SharedTool.IsGetMainCamera())
            return;

        DrawAll(mDrawCommandBufferFun);
    }

    void DrawAll(DrawMeshFun fun)
    {
        foreach (var mf in meshFilters)
            DrawMesh(mf,fun);

        foreach (var smr in skinnedMeshRenderers)
            DrawSkin(smr, fun);
    }

    void DrawSkin(SkinnedMeshRenderer skinnedMeshRenderer, DrawMeshFun fun)
    {
        var mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(mesh);
        var matrix = skinnedMeshRenderer.transform.localToWorldMatrix;
        fun(mesh, ref matrix);
    }

    void DrawMesh(MeshFilter meshFilter, DrawMeshFun fun) {
        var mesh = meshFilter.mesh;
        var matrix = meshFilter.transform.localToWorldMatrix;
        fun(mesh, ref matrix);
    }

    void Update()
    {
        DrawAll(mDrawFun);
    }


}
