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
    Material materialDrawBehindTheWall;
    Material materialDrawMask;
    RenderTexture depth;
    private RenderBehindTheWallCommandBuffer()
    {
        cam = Camera.main;
    }

    //(沒在使用了)
    //有了Mask就不需要了
    void DrawDepthTexture()
    {
        //這裡用RenderTextureFormat.Depth就看不到效果
        depth = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.RFloat);
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

    RenderBehindTheWallCamera.QueueOrderForMainBody queueOrderForMainBody;
    public void SetRequired(Material materialDrawBehindTheWall, Material materialDrawMask, RenderBehindTheWallCamera.QueueOrderForMainBody queueOrderForMainBody)
    {
        this.materialDrawBehindTheWall = materialDrawBehindTheWall;
        this.materialDrawMask = materialDrawMask;
        this.queueOrderForMainBody = queueOrderForMainBody;

        //強制設成Forward Rendering
        if (queueOrderForMainBody == RenderBehindTheWallCamera.QueueOrderForMainBody.MainBodyAfterMask)
            cam.renderingPath = RenderingPath.Forward;
    }

    public RenderBehindTheWallCamera.QueueOrderForMainBody GetQueueOrderForMainBody()
    {
        return queueOrderForMainBody;
    }

    public void DrawBehindTheWall(Mesh mesh,ref Matrix4x4 matrix)
    {
        var subCount = mesh.subMeshCount;
        var buf= new CommandBuffer();
        buf.name = "Draw BehindTheWall";
        for (var i = 0; i < subCount; i++)
        {
            buf.DrawMesh(mesh, matrix, materialDrawBehindTheWall, i);
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
