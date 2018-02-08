using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPlayer : MonoBehaviour, FactoryPlugin
{

    public EndlessCorridorManager ecManager;
    [SerializeField]
    float scale = 1;
    public void doIt(GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}
