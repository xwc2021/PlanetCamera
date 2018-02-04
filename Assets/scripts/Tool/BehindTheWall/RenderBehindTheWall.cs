using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//OnWillRenderObject需要加在有MeshFilter或SkinnedMeshRenderer元件的GameObject身上才會觸發!!
public class RenderBehindTheWall : MonoBehaviour {
    
    MeshFilter[] meshFilters;
    SkinnedMeshRenderer[] skinnedMeshRenderers;

    void Awake()
    {
        //會往下層節點尋找
        meshFilters = GetComponentsInChildren<MeshFilter>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    //OnBecameVisible只有第1個camera看到物件才會觸發
    //OnWillRenderObject需要加在有MeshFilter或SkinnedMeshRenderer元件的GameObject身上才會觸發!!
    void OnWillRenderObject()
    {
        if (!SharedTool.IsGetMainCamera())
            return;

        foreach (var mf in meshFilters)
            DrawMesh(mf);

        foreach (var smr in skinnedMeshRenderers)
            DrawSkin(smr);
    }

    void DrawSkin(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        var mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(mesh);
        var matrix = skinnedMeshRenderer.transform.localToWorldMatrix;
        RenderBehindTheWallCommandBuffer.getInstance().Draw(mesh, ref matrix);
    }

    void DrawMesh(MeshFilter meshFilter) {
        var mesh = meshFilter.mesh;
        var matrix = meshFilter.transform.localToWorldMatrix;
        RenderBehindTheWallCommandBuffer.getInstance().Draw(mesh, ref matrix);
    }


}
