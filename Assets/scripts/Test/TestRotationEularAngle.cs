using UnityEngine;
using System.Collections;

public class TestRotationEularAngle : MonoBehaviour
{

    public float degree = 0;
    public bool dynamic = false;
    public bool global = true;
    public Transform target;

    // Use this for initialization
    void Start()
    {
        backup = transform.rotation;
    }
    Quaternion backup;

    // Update is called once per frame
    void Update()
    {

        Quaternion rotQ = Quaternion.Euler(degree,0,0);
        

        if (!dynamic)
        {
            if (global)
            {
                transform.rotation = rotQ * backup;
            }
            else
                transform.rotation = backup * rotQ;
        }
        else
        {
            if (global)
            {
                transform.rotation = rotQ * transform.rotation;
            }
            else
                transform.rotation = transform.rotation * rotQ;
        }



    }
}
