using UnityEngine;
using System.Collections;

//舊方法，已改成使用CameraPivot
public class FlowCamera : MonoBehaviour {

    public Transform target;
    public float speedTranslate=1;
    public float speedRotation = 1;
    public bool follow = false;
    // Use this for initialization
    void Start () {
	
	}
	
	void LateUpdate() {
        if (follow)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speedTranslate * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speedRotation * Time.deltaTime);
        }
        else {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
