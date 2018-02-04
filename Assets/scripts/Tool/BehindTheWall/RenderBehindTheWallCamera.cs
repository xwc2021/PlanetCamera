using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderBehindTheWallCamera : MonoBehaviour {

    [SerializeField]
    Material m_Material;

    void Awake()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.setMaterial(m_Material);
    }

    void OnPostRender()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.clearCommand();
    }
}
