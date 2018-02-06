using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderBehindTheWallCommandBuffer
{
    private static RenderBehindTheWallCommandBuffer instance;
    public static RenderBehindTheWallCommandBuffer getInstance()
    {
        if (instance == null)
            instance = new RenderBehindTheWallCommandBuffer();

        return instance;
    }

    Camera cam;
    Material materialUseDepthTexture;
    Material materialDrawMask;
    RenderTexture depth;
    private RenderBehindTheWallCommandBuffer()
    {
        cam = Camera.main;

        //這裡用RenderTextureFormat.Depth就看不到效果
        depth = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.RFloat);
        depth.Create();

        var depthID = new RenderTargetIdentifier(depth);

        var bufDrawDepth = new CommandBuffer();
        bufDrawDepth.name = "Draw Depth Texture";

        //[方便測式用]這裡強制設成Forward Rendering
        cam.renderingPath = RenderingPath.Forward;
        //這樣如果是RenderingPath.UsePlayerSettings，也能取出確切的RenderPath
        var src = cam.actualRenderingPath == RenderingPath.Forward ? BuiltinRenderTextureType.Depth : BuiltinRenderTextureType.ResolvedDepth;
        bufDrawDepth.Blit(src, depthID);
        bufDrawDepth.SetGlobalTexture("_DepthTexture", depthID);

        cam.AddCommandBuffer(CameraEvent.AfterSkybox, bufDrawDepth);
    }

    public void setMaterial(Material materialUseDepthTexture, Material materialDrawMask)
    {
        this.materialUseDepthTexture = materialUseDepthTexture;
        this.materialDrawMask = materialDrawMask;
    }

    public void DrawBehindTheWallUseDepthTexture(Mesh mesh, ref Matrix4x4 matrix)
    {
        DrawBehindTheWall(mesh,ref matrix);
    }


    public void DrawBehindTheWall(Mesh mesh,ref Matrix4x4 matrix)
    {
        var subCount = mesh.subMeshCount;
        var buf= new CommandBuffer();
        buf.name = "Draw BehindTheWall";
        for (var i = 0; i < subCount; i++)
        {
            buf.DrawMesh(mesh, matrix, materialUseDepthTexture, i);
        }

        cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, buf);
    }

    public void clearCommand()
    {
        cam.RemoveCommandBuffers(CameraEvent.BeforeForwardAlpha);
    }

    public void DrawMask(Mesh mesh, ref Matrix4x4 matrix)
    {
        var subCount = mesh.subMeshCount;
        for (var i = 0; i < subCount; i++)
        {
            Graphics.DrawMesh(mesh, matrix, materialDrawMask, 0,cam,i);
        }
    }

}
