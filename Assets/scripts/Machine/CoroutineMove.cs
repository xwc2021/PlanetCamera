using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineMove : MonoBehaviour {

    [SerializeField]
    Transform target;

    [SerializeField]
    float totlaTime = 2;

    float startTime;
    Vector3 from, to;

    private void Start()
    {
        startMove();
    }

    //call this if you want to lerp
    void startMove()
    {
        from = transform.position;
        to = target.position;
        startTime = Time.time;
        StartCoroutine(moving());
    }

    void swap()
    {
        Vector3 temp = from;
        from = to;
        to = temp;

        startTime = Time.time;
    }

    //lerps from a to b based on time and writes to value
    IEnumerator moving()
    {
        while (true)
        {
            float t = (Time.time - startTime) / totlaTime;
            transform.position =Vector3.Lerp(from, to, Mathf.SmoothStep(0, 1, t));

            if (t >= 1)
                swap();

            //http://www.cnblogs.com/wangchengfeng/p/3724377.html
            yield return new WaitForFixedUpdate();
            //yield return null;
        }
    }
}
