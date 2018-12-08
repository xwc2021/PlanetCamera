using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RayPeaker : MonoBehaviour
{
    public Transform to;
    // Update is called once per frame
    void Update()
    {
        // if (!Application.isPlaying)
        {
            // code executed in edit mode
            if (to != null)
            {
                var from = this.transform.position;
                var dir = (to.position - from).normalized;
                Debug.DrawLine(this.transform.position, from + dir * 10000.0f);
            }

        }
    }
}
