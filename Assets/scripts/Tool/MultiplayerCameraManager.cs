using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerCameraManager : NetworkBehaviour {

    public GameObject refCameraPivot;
    public MonoBehaviour planetMovable;
    public MonoBehaviour  planetPlayerController;
    public override void OnStartLocalPlayer()
    {
        refCameraPivot.SetActive(true);
        planetMovable.enabled = true;
        planetPlayerController.enabled = true;
        print("active Camera GameObject and MonoBehaviour");

    }
}
