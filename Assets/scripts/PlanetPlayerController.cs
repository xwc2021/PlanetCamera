using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface FollowCameraBehavior
{
    void rotateByAxis(float angle, Vector3 axis);
}

public interface MoveController
{
    Vector3 getMoveForce();
    bool doJump();
}

public class PlanetPlayerController : MonoBehaviour, MoveController
{

    public Transform laddingPlanet;
    FollowCameraBehavior followCameraBehavior;
    public MonoBehaviour followCameraBehaviorSocket;
    public bool adjustCameraWhenMove = true;
    InputProxy inputProxy;
    public MonoBehaviour inputPorxySocket;
    Transform m_Cam;

    Vector3 previouPosistion;

    // Use this for initialization
    void Start () {
        previouPosistion = transform.position;

        if (followCameraBehaviorSocket != null)
            followCameraBehavior = followCameraBehaviorSocket as FollowCameraBehavior;

        //print("cameraBehavior="+cameraBehavior);

        if (inputPorxySocket != null)
            inputProxy = inputPorxySocket as InputProxy;

        m_Cam = Camera.main.transform;
    }

    void Update()
    {
        //如果在FixedUpdate做會抖動
        if (adjustCameraWhenMove)
            doAdjust();
    }

    void doAdjust()
    {
        //如果位置有更新，就更新FlowPoint
        //透過headUp和向量(nowPosition-previouPosistion)的外積，找出旋轉軸Z
        //用A軸來旋轉CameraPivot

        Vector3 diffV = transform.position - previouPosistion;
        //Vector3 Z = Vector3.Cross(headUp, diffV);
        Vector3 Z = Vector3.Cross(transform.up, diffV);

        //算出2個frame之間在planet上移動的角度差
        Vector3 from = (previouPosistion - laddingPlanet.position).normalized;
        Vector3 to = (transform.position - laddingPlanet.position).normalized;
        float cosValue = Vector3.Dot(from, to);
        float rotDegree = Mathf.Acos(cosValue) * Mathf.Rad2Deg;

        if (followCameraBehavior != null)
            followCameraBehavior.rotateByAxis(rotDegree, Z);

        previouPosistion = transform.position;
    }

    public Vector3 getMoveForce()
    {
        //取得輸入
        Vector2 hv = inputProxy.getHV();
        float h = hv.x;
        float v = hv.y;

        if (h != 0 || v != 0)
        {
            //由camera向方計算出角色的移動方向
            Vector3 controllForce = h * m_Cam.right + v * m_Cam.up;
            return controllForce;
        }

        return Vector3.zero;
    }

    public bool doJump()
    {
        return inputProxy.pressJump();
    }
}
