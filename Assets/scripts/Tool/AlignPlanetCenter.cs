using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignPlanetCenter : MonoBehaviour {

    public Transform planet;
	
	public void align () {
        Vector3 up =transform.position - planet.position;
        Vector3 right = Vector3.Cross(up, transform.forward);
        Vector3 forward = Vector3.Cross(right, up);

        transform.rotation = Quaternion.LookRotation(forward,up);
	}
	
	
}
