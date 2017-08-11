using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMove : MonoBehaviour {

    public bool useMovePositon=true;
    Vector3 recordPos;
    Rigidbody rigid;
    public float speed = 5;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        recordPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate () {
        Vector3 newPos = recordPos + 5 * Mathf.Sin(Time.fixedTime) * transform.right;
        if (useMovePositon)
            //rigid.position = newPos;
            rigid.MovePosition(newPos);
        else
            //transform.position= newPos;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.fixedDeltaTime * speed);
            //rigid.position = newPos;
    }
}
