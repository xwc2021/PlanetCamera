using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryGameObject : MonoBehaviour {

    public GameObject lockstitch;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(lockstitch);
    }
}
