using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByAxis2 : MonoBehaviour
{

    public Transform[] target;
    // Use this for initialization
    void Start()
    {

        Vector3 sum = Vector3.zero;
        foreach (Transform obj in target)
        {
            sum = sum + obj.up;
        }
        sum = sum / target.Length;
        Vector3 averageN = sum.normalized;
        print(averageN);

        Vector3 targetNormal = averageN;
        Vector3 nowNormal = transform.up;
        Vector3 axis = Vector3.Cross(nowNormal, targetNormal);
        float dotValue = Vector3.Dot(nowNormal, targetNormal);
        float degree = Mathf.Rad2Deg * Mathf.Acos(dotValue);
        print(degree);

        Quaternion rot = Quaternion.AngleAxis(degree, axis);
        newRot = rot * transform.rotation;

    }
    Quaternion newRot;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = newRot;
    }
}
