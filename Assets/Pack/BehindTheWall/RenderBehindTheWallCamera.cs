using UnityEngine;
public class RenderBehindTheWallCamera : MonoBehaviour
{
    public enum QueueOrderForMainBody { MainBodyAfterBehindWall = 2999 }

    //[SerializeField]
    QueueOrderForMainBody queueOrderForMainBody = QueueOrderForMainBody.MainBodyAfterBehindWall;

    [SerializeField]
    Material m_MaterialDrawBehindTheWall;

    void Awake()
    {
        var instance = RenderBehindTheWallCommandBuffer.getInstance();
        instance.SetRequired(m_MaterialDrawBehindTheWall, queueOrderForMainBody);
    }
}