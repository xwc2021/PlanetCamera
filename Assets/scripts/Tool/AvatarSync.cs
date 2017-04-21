using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSync : MonoBehaviour {

    public Transform stage;
    public Transform player;
    public Transform[] avatar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector3 localPos =stage.transform.InverseTransformPoint(player.position);

        //Qworld = Qstage * Qlocal;
        //(Qstage)^-1*Qworld= Qlocal;
        Quaternion Qlocal = Quaternion.Inverse(stage.transform.rotation) * player.transform.rotation;

        foreach (Transform t in avatar)
        {
            if (t != null)
            {
                t.localPosition = localPos;
                t.localRotation = Qlocal;
            }
            
        }

    }
}
