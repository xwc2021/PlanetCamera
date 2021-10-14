using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnCollisionEnterPrintCollision : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        print("collision = "+collision.gameObject.name+" colider = "+collision.collider.name);
    }
}
