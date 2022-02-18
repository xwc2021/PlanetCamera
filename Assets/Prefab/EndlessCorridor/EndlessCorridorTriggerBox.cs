using UnityEngine;
public class EndlessCorridorTriggerBox : MonoBehaviour
{
    public EndlessCorridorManager ecManager;
    public EndlessCorridorHolder ec;
    public Transform borderLimit;

    static float smaller_scaleA = 1.0f;
    static float smaller_scaleB = 0.5f;

    static float bigger_scaleA = 2.0f;
    static float bigger_scaleB = 1.0f;

    float scaleA;
    float scaleB;

    public bool doRescale = true;

    float borderValue;
    private void Awake()
    {
        borderValue = Mathf.Abs(borderLimit.localPosition.x);
    }

    //localRatio在A是0，在B是1
    float getLocalRatio(Collider other)
    {
        Vector3 localPos = transform.InverseTransformPoint(other.transform.position);
        float localValue = Mathf.Abs(localPos.x);

        float clampValue = Mathf.Clamp(localValue, 0, borderValue);
        float ratio = clampValue / borderValue;
        return ratio;
    }

    void scaleIt(Collider other)
    {
        float r = getLocalRatio(other);
        float scale = Mathf.Lerp(scaleA, scaleB, r);

        ecManager.player.doScale(scale);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != TagDefined.Player)
            return;

        if (!doRescale)
            return;

        scaleIt(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != TagDefined.Player)
            return;

        if (!doRescale)
            return;

        float playerScale = other.transform.localScale.y;
        if (playerScale < 0.6f)
        {
            //進行縮小修正
            ecManager.CallWorldReSacle(2.0f);
            print("縮小修正");
        }
        else if (playerScale > 1.8f)
        {
            //進行放大修正
            ecManager.CallWorldReSacle(0.5f);
            print("放大修正");
        }
        else if (playerScale < 0.91f)
            ecManager.player.resetScale();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != TagDefined.Player)
            return;

        //更新地板
        ecManager.updateList(ec.ListIndex);

        if (!doRescale)
            return;

        float localRatio = getLocalRatio(other);
        if (localRatio < 0.5)//變小
        {
            scaleA = smaller_scaleA;
            scaleB = smaller_scaleB;
        }
        else
        {
            scaleA = bigger_scaleA;
            scaleB = bigger_scaleB;
        }
    }
}