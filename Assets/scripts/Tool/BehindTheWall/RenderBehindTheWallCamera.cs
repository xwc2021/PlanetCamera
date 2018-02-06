using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderBehindTheWallCamera : MonoBehaviour {

    [SerializeField]
    Material m_MaterialUseDepthTexture;

    [SerializeField]
    Material m_MaterialDrawMask;

    void Awake()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.setMaterial(m_MaterialUseDepthTexture, m_MaterialDrawMask);
    }

    void OnPostRender()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.clearCommand();
    }
}
