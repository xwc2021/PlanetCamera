using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	// Use this for initialization
	void Start () {
		rigid = this.GetComponent<Rigidbody2D> ();

		onAir = false;
		
	}

	bool onAir;

	Rigidbody2D rigid;

	public float move_v;
	float v_scale =1.0f;


	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.transform.position.y < transform.position.y) {
			onAir = false;

			//if(!hold_moving)
			//rigid.velocity  = new Vector2 ( 0,0);
		}
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKey ("s")) {
			rigid.velocity = new Vector2 (-move_v*v_scale, rigid.velocity.y);
		}

		else if (Input.GetKey ("f")) {
			rigid.velocity = new Vector2 (move_v*v_scale, rigid.velocity.y);
		}

        if (Input.GetKeyDown("space"))
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 5);
            onAir = true;
        }

        if (Input.GetKey ("j")) {
            v_scale = 3.0f;
		} else if (Input.GetKeyUp ("j")) {
			v_scale = 1.0f;
		}

		print (rigid.velocity);		
	}
}
