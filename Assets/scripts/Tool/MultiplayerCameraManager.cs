using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerCameraManager : NetworkBehaviour {

    public GameObject refCameraGameObject;
    public override void OnStartLocalPlayer()
    {
        refCameraGameObject.SetActive(true);
        print("active Camera GameObject");

    }
}
