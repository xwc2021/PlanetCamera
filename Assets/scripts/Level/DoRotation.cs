using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoRotation : MonoBehaviour {

    public float speed = 10;
    float nowDegree = 0;
    private void FixedUpdate()
    {
        nowDegree = nowDegree + speed * Time.fixedDeltaTime;
        nowDegree = nowDegree % 360;
        transform.rotation = Quaternion.Euler(nowDegree, 0, 0);
    }
}
