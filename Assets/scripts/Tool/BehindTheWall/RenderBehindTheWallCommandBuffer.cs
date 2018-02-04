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

    CommandBuffer bufDrawDepth;
    Camera cam;
    Material material;
    private RenderBehindTheWallCommandBuffer()
    {
        bufDrawDepth = new CommandBuffer();
        bufDrawDepth.name = "Draw Depth Texture";

        cam = Camera.main;
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
