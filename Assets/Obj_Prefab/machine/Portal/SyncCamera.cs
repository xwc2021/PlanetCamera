using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCamera : MonoBehaviour {

    public Camera playerCamera;
    public Transform syncCamera;
    public Material mirrorMaterial;
    Matrix4x4 P;
    // Use this for initialization
    void Start () {
        CameraPivot cameraPivot = (CameraPivot)FindObjectOfType(typeof(CameraPivot));
        playerCamera = cameraPivot.GetComponentInChildren<Camera>();
        P = playerCamera.projectionMatrix;
    }

    void Update () {

        Vector3 localPos = transform.InverseTransformPoint(playerCamera.transform.position);

        //Qworld = Qstage * Qlocal;
        //(Qstage)^-1*Qworld= Qlocal;
        Quaternion Qlocal = Quaternion.Inverse(transform.rotation) * playerCamera.transform.rotation;

        syncCamera.localPosition = localPos;
        syncCamera.localRotation = Qlocal;

        Matrix4x4 V = playerCamera.worldToCameraMatrix;
        mirrorMaterial.SetMatrix("_mirror_camera_vp", P * V);
    }
}
