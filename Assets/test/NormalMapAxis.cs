using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMapAxis : MonoBehaviour {

    public Transform objBN;

    public Vector4 T;
    public Vector3 N;
    public Vector3 BN;

    public void initByData(Vector4 tangent,Vector3 normal)
    {
        N = normal;
        T = tangent;

        BN = Vector3.Cross(N, T) * T.w;

        transform.rotation=Quaternion.LookRotation(T, N);

        if (T.w == -1)
            objBN.localPosition = new Vector3(-2.5f,0,0);
        else
            objBN.localPosition = new Vector3(2.5f, 0, 0);
    }


    // Update is called once per frame
    void Update () {
		
	}
}
