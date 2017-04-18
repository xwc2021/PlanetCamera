using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoWorldScale : MonoBehaviour {

    public MonoBehaviour socket;
    EndlessCorridorManager ecManager;

    void Start()
    {
        if (socket != null)
            ecManager = socket as EndlessCorridorManager;
    }
    void callWorldReSacle(float scaleValue)
    {
        ecManager.worldReSacle(scaleValue);
    }
}
