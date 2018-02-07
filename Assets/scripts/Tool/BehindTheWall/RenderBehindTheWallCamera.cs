using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RenderBehindTheWallCamera : MonoBehaviour {

    public enum QueueOrderForMainBody { MainBodyAfterMask = 2050, MainBodyAfterBehindWall = 2999 }

    //[SerializeField]
    //[Tooltip("MainBodyAfterMask是方法一；MainBodyAfterBehindWall是方法二")]
    QueueOrderForMainBody queueOrderForMainBody = QueueOrderForMainBody.MainBodyAfterBehindWall;

    [SerializeField]
    Material m_MaterialDrawBehindTheWallUseMask;

    [SerializeField]
    Material m_MaterialDrawBehindTheWall;

    [SerializeField]
    Material m_MaterialDrawMask;

    void Awake()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();

        var materail = queueOrderForMainBody == QueueOrderForMainBody.MainBodyAfterMask ?
                                            m_MaterialDrawBehindTheWallUseMask :
                                            m_MaterialDrawBehindTheWall;

        instance.SetRequired(materail, m_MaterialDrawMask,queueOrderForMainBody);
    }

    void OnPostRender()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.clearCommand();
    }
}
