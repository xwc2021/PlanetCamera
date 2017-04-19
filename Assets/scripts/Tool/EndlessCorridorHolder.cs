using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorHolder : MonoBehaviour {

    public Transform headDummy;
    public Transform tailDummy;
    public EndlessCorridorTriggerBox triggerbox;


    public void initEC(int index, EndlessCorridorManager ecManager)
    {
        ListIndex = index;
        triggerbox.ecManager = ecManager;
    }

    int listIndex;
    public int ListIndex
    {
        
        get { return listIndex; }
        set {
            listIndex = value;
            this.name = "EndlessCorridorHolder_" + listIndex.ToString("D3");
        }
    }

    public Transform getHeadDummy()
    {
        return headDummy;
    }

    public Transform getTailDummy()
    {
        return tailDummy;
    }

    public float getGlobalScale()
    {
        return transform.lossyScale.x;
    }
}
