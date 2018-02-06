using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RenderBehindTheWallCamera : MonoBehaviour {

    public enum QueueOrderForMainBody { GeometryAfterMask = 2050, TransparentAfterBehindWall = 2999 }

    //[SerializeField]
    //[Tooltip("GeometryAfterMask是方法一；TransparentAfterBehindWall是方法二")]
    QueueOrderForMainBody queueOrderForMainBody = QueueOrderForMainBody.TransparentAfterBehindWall;

    [SerializeField]
    Material m_MaterialDrawBehindTheWallUseDepthTexture;

    [SerializeField]
    Material m_MaterialDrawBehindTheWall;

    [SerializeField]
    Material m_MaterialDrawMask;

    void Awake()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();

        var materail = queueOrderForMainBody == QueueOrderForMainBody.GeometryAfterMask?
                                            m_MaterialDrawBehindTheWallUseDepthTexture:
                                            m_MaterialDrawBehindTheWall;

        instance.SetRequired(materail, m_MaterialDrawMask,queueOrderForMainBody);
    }

    void OnPostRender()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.clearCommand();
    }
}
