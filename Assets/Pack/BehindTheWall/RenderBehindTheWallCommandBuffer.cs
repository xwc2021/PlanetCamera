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
    RenderTexture depth;
    private RenderBehindTheWallCommandBuffer()
    {
        cam = Camera.main;
    }

    RenderBehindTheWallCamera.QueueOrderForMainBody queueOrderForMainBody;
    public void SetRequired(Material materialDrawBehindTheWall, RenderBehindTheWallCamera.QueueOrderForMainBody queueOrderForMainBody)
    {
        this.materialDrawBehindTheWall = materialDrawBehindTheWall;
        this.queueOrderForMainBody = queueOrderForMainBody;
    }

    public RenderBehindTheWallCamera.QueueOrderForMainBody GetQueueOrderForMainBody()
    {
        return queueOrderForMainBody;
    }

    public void DrawBehindTheWall(Mesh mesh, ref Matrix4x4 matrix)
    {
        GraphicsDrawMesh(mesh, ref matrix, materialDrawBehindTheWall);
    }


    public void clearCommand()
    {
        if (cam)
            cam.RemoveCommandBuffers(CameraEvent.BeforeForwardAlpha);
    }

    public void GraphicsDrawMesh(Mesh mesh, ref Matrix4x4 matrix, Material material)
    {
        var subCount = mesh.subMeshCount;
        for (var i = 0; i < subCount; i++)
        {
            Graphics.DrawMesh(mesh, matrix, material, 0, cam, i);
        }
    }

}
