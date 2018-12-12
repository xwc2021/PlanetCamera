using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stitching : MonoBehaviour
{

    Material material;

    public Vector2 uDir;
    public Vector2 vDir;
    public void setHeightTexture(RenderTexture height, RenderTexture neighborHeight)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        material.SetVector("_uvDir", new Vector4(uDir.x, uDir.y, vDir.x, vDir.y));

        material.SetTexture("_HeightTex", height);
        material.SetTexture("_NeighborHeightTex", neighborHeight);
    }
}
