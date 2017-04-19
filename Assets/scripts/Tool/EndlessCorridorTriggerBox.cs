using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorTriggerBox : MonoBehaviour {

    public EndlessCorridorManager ecManager;
    public EndlessCorridorHolder ec;
    public Transform borderLimit;

    static float smaller_scaleA = 1.0f;
    static float smaller_scaleB = 0.5f;

    static float bigger_scaleA = 2.0f;
    static float bigger_scaleB = 1.0f;

    float scaleA;
    float scaleB;

    float borderValue;
    private void Awake()
    {
        borderValue= Mathf.Abs(borderLimit.localPosition.x);
    }

    float getLocalRatio(Collider other)
    {
        Vector3 localPos = transform.InverseTransformPoint(other.transform.position);
        float localValue = Mathf.Abs(localPos.x);

        float checkValue = Mathf.Max(0, localValue);
        checkValue = Mathf.Min(borderValue, localValue);
        float ratio = checkValue / borderValue;

        print(ratio);
        return ratio;
    }

    void scaleIt(Collider other)
    {
        float r = getLocalRatio(other);
        float scale = Mathf.Lerp(scaleA, scaleB, r);
        ecManager.scaleTarget.doScale(scale);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "player")
        {

            scaleIt(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "player")
        {
            print("exit");
            scaleIt(other);

            float playerScale = other.transform.localScale.y;
            if (playerScale<0.6)
            {
                //進行縮小修正
                other.transform.localScale = Vector3.one;
                ecManager.worldReSacle(0.5f);
                print("縮小修正");
            }
            else if (playerScale>1.9)
            {
                //進行放大修正
                other.transform.localScale = Vector3.one;
                ecManager.worldReSacle(2.0f);
                print("放大修正");
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //更新地板
        ecManager.updateList(ec.ListIndex);

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
