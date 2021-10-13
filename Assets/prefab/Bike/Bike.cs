using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bike : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.enabled = false;
        DrawInstance.getWorker(DrawInstance.KEY_bike).pushTrasform(transform);
    }
}
