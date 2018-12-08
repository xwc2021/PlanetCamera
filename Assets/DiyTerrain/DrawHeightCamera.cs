using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHeightCamera : MonoBehaviour
{
    public Camera usingCamera;
    public SphereTerrain sphereTerrain;
    public MeshRenderer targetPlane;

    RenderTexture keepTexture;
    RenderTexture nowRenderTexture;
    int tIndex;
    void Start()
    {
        var descriptor = new RenderTextureDescriptor(1024, 1024, RenderTextureFormat.ARGBHalf);
        keepTexture = new RenderTexture(descriptor);
        nowRenderTexture = new RenderTexture(descriptor);

        usingCamera.targetTexture = nowRenderTexture;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, keepTexture);

        sphereTerrain.updateHeightTexture(keepTexture);
        targetPlane.material.SetTexture("_MainTex", keepTexture);
    }

}
