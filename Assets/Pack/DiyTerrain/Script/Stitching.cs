using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stitching : MonoBehaviour
{

    Material material;

    public Vector2 neibhborU;
    public Vector2 neibhborV;
    public Vector2 neibhborOriginal;
    public void setHeightTexture(RenderTexture height, RenderTexture neighborHeight)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        material.SetVector("_neibhborUV", new Vector4(neibhborU.x, neibhborU.y, neibhborV.x, neibhborV.y));
        material.SetVector("_neibhborOriginal", new Vector4(neibhborOriginal.x, neibhborOriginal.y, 0.0f, 0.0f));

        material.SetTexture("_HeightTex", height);
        material.SetTexture("_NeighborHeightTex", neighborHeight);
    }
}
