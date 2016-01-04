using UnityEngine;
using System.Collections;

public class FlowCamera : MonoBehaviour {

    public Transform target;
    public float speedTranslate=1;
    public float speedRotation = 1;
    public bool follow = false;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate() {
        if (follow)
        {
            //這種follow方式不太好
            transform.position = Vector3.Lerp(transform.position, target.position, speedTranslate * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speedRotation * Time.deltaTime);
        }
        else {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
