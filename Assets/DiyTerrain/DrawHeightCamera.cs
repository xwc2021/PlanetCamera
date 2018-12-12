using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DrawHeightCamera : MonoBehaviour
{
    public Camera usingCamera;
    public SphereTerrain sphereTerrain;
    public MeshRenderer targetPlane;

    public RenderTexture keepTexture;
    RenderTexture nowRenderTexture;

    public Stitching stitchingUp;
    public Stitching stitchingDown;
    public Stitching stitchingLeft;
    public Stitching stitchingRight;

    void Awake()
    {
        var descriptor = new RenderTextureDescriptor(1024, 1024, RenderTextureFormat.RFloat);
        keepTexture = new RenderTexture(descriptor);
        nowRenderTexture = new RenderTexture(descriptor);

        usingCamera.targetTexture = nowRenderTexture;
        print("init");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, keepTexture);

        sphereTerrain.updateHeightTexture(keepTexture);
        targetPlane.material.SetTexture("_MainTex", keepTexture);
    }

    // https://gist.github.com/AlexanderDzhoganov/d795b897005389071e2a
    public void DumpRenderTexture(string pngOutPath)
    {
        var saveName = pngOutPath + transform.parent.parent.name + ".png";
        RenderTexture rt = keepTexture;
        var oldRT = RenderTexture.active;

        var tex = new Texture2D(rt.width, rt.height);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        File.WriteAllBytes(saveName, tex.EncodeToPNG());
        RenderTexture.active = oldRT;
    }
}
