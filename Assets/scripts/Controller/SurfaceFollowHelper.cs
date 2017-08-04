using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetMovable))]
public class SurfaceFollowHelper : MonoBehaviour {

    public CameraPivot cameraPivot;
    PlanetMovable planetMovable;
    Vector3 previousGroundUp;

    private void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        previousGroundUp = transform.up;
    }

    // Update is called once per frame
    void FixedUpdate () {
        //從Update移到FixedUpdate
        //因為無法保證FixedUpdate在第1個frame一定會執行到
        doAdjustByGroundUp();
    }

    void doAdjustByGroundUp()
    {

        //如果位置有更新，就更新FlowPoint
        //透過groundUp和向量(nowPosition-previouPosistion)的外積，找出旋轉軸Z

        Vector3 groundUp = planetMovable.GroundUp;

        Vector3 Z = Vector3.Cross(previousGroundUp, groundUp);
        //Debug.DrawLine(transform.position, transform.position + Z * 16, Color.blue);
        //Debug.DrawLine(transform.position, transform.position + previousGroundUp * 16, Color.red);
        //Debug.DrawLine(transform.position, transform.position + groundUp * 16, Color.green);

        //算出2個frame之間在planet上移動的角度差
        float cosValue = Vector3.Dot(previousGroundUp, groundUp);

        //http://answers.unity3d.com/questions/778626/mathfacos-1-return-nan.html
        //上面說Dot有可能會>1或<-1
        cosValue = Mathf.Max(-1.0f, cosValue);
        cosValue = Mathf.Min(1.0f, cosValue);

        float rotDegree = Mathf.Acos(cosValue) * Mathf.Rad2Deg;
        //print("rotDegree=" + rotDegree);

        if (float.IsNaN(rotDegree))
        {
            print("IsNaN");
            return;
        }

        float threshold = 0.1f;
        if (rotDegree > threshold)
        {
            //print("rotDegree=" + rotDegree);
            Quaternion q = Quaternion.AngleAxis(rotDegree, Z);

            cameraPivot.setSurfaceRotate(true, q);
            previousGroundUp = groundUp;//有轉動才更新
        }
    }
}
