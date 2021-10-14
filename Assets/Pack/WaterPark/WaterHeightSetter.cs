using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHeightSetter : MonoBehaviour
{

    [SerializeField]
    MeshRenderer meshRender;

    [SerializeField]
    Transform target;

    public void updateWater(float h)
    {
        if (h <= 0.1f)
            meshRender.enabled = false;
        else
        {
            meshRender.enabled = true;
            target.localScale = new Vector3(1, h, 1);
        }
    }
}
