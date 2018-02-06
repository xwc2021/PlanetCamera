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
    Material material;
    RenderTexture depth;
    private RenderBehindTheWallCommandBuffer()
    {
        cam = Camera.main;

        //這裡用RenderTextureFormat.Depth就看不到效果
        depth = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.RFloat);
        Debug.Log(cam.pixelHeight);
        depth.Create();

        var depthID = new RenderTargetIdentifier(depth);

        var bufDrawDepth = new CommandBuffer();
        bufDrawDepth.name = "Draw Depth Texture";

        //這樣如果是RenderingPath.UsePlayerSettings，也能取出確切的RenderPath
        var src = cam.actualRenderingPath == RenderingPath.Forward ? BuiltinRenderTextureType.Depth : BuiltinRenderTextureType.ResolvedDepth;
        bufDrawDepth.Blit(src, depthID);
        bufDrawDepth.SetGlobalTexture("_DepthTexture", depthID);

        cam.AddCommandBuffer(CameraEvent.AfterSkybox, bufDrawDepth);
    }

    public void setMaterial(Material material)
    {
        this.material = material;
    }
    
    public void Draw(Mesh mesh,ref Matrix4x4 matrix)
    {
        var subCount = mesh.subMeshCount;
        var buf= new CommandBuffer();
        buf.name = "Draw BehindTheWall";
        for (var i = 0; i < subCount; i++)
        {
            buf.DrawMesh(mesh, matrix, material,i);
        }

        cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, buf);
    }

    public void clearCommand()
    {
        cam.RemoveCommandBuffers(CameraEvent.BeforeForwardAlpha);
    }

}
