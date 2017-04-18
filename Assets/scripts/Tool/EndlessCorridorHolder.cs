using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorHolder : MonoBehaviour {

    public Transform headDummy;
    public Transform tailDummy;

    public Transform getHeadDummy()
    {
        return headDummy;
    }

    public Transform getTailDummy()
    {
        return tailDummy;
    }
}
