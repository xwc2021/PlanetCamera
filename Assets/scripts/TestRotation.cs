using UnityEngine;
using System.Collections;

public class TestRotation : MonoBehaviour {

    public float degree = 0;
    public bool dynamic = false;
    public bool pre= true;
    public bool bufferFlyEffect = true;
    public Transform target;

	// Use this for initialization
	void Start () {
        backup = transform.rotation;
        backupRight = transform.right;
    }
    Quaternion backup;
    Vector3 backupRight;
    
    // Update is called once per frame
    void Update () {

        Quaternion rotQ = Quaternion.identity;
        //這樣在!dynamic時累積誤差會產生蝴蝶效應(當degree太大時，沒多久就會開始晃動)
        if (bufferFlyEffect)
            rotQ =  Quaternion.AngleAxis(degree, transform.right);
        else
            rotQ = Quaternion.AngleAxis(degree, backupRight);

        if (!dynamic)
        {
            if (pre)
            {
                transform.rotation = rotQ * backup;
            }  
            else
                transform.rotation = backup * rotQ;
        }
        else
        {
            if (pre)
            {
                Debug.DrawLine(transform.position, transform.position+transform.right*20, Color.green);
                Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotQ, Vector3.one);

                Vector3 dir = target.position - transform.position;
                Debug.DrawRay(transform.position, dir, Color.blue);

                float cosValue =Vector3.Dot(transform.right, dir.normalized);
                print(Mathf.Acos(cosValue)*Mathf.Rad2Deg);

                dir = m * dir;
                //如果旋轉之後
                Debug.DrawRay(transform.position, dir, Color.red);

                transform.rotation = rotQ * transform.rotation;
            }
            else
                transform.rotation = transform.rotation * rotQ;
        }

        
   
    }
}
