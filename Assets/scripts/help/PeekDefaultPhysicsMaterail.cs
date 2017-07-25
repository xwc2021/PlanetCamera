using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekDefaultPhysicsMaterail : MonoBehaviour {

    Collider collider;
    public float dynamicFriction;
    public float staticFriction;
    public PhysicMaterialCombine frictionCombine;
    public PhysicMaterialCombine bounceCombine;
    // Use this for initialization
    void Start () {
        collider = GetComponent<Collider>();
        dynamicFriction=collider.material.dynamicFriction;
        staticFriction = collider.material.staticFriction;
        frictionCombine = collider.material.frictionCombine;
        bounceCombine = collider.material.bounceCombine;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
