using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerCameraManager : NetworkBehaviour {

    public GameObject refCameraPivot;
    public MonoBehaviour planetMovable;
    public PlanetPlayerController planetPlayerController;

    Animator animator;
    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnStartLocalPlayer()
    {
        refCameraPivot.SetActive(true);
        planetMovable.enabled = true;
        planetPlayerController.enabled = true;
        planetPlayerController.getCamera();

        print("active Camera GameObject and MonoBehaviour");

    }

    [Command]
    //不知為何Network Transform不會Sync rotation
    //Client呼叫Server
    public void CmdSyncAnimatorAndRot(bool moving,bool doJump,bool onAir,Quaternion rot)
    {
        RpcSyncAnimatorAndRot(moving, doJump, onAir, rot);
    }

    [ClientRpc]
    //Server呼叫Client
    void RpcSyncAnimatorAndRot(bool moving, bool doJump, bool onAir, Quaternion rot)
    {
        if (isLocalPlayer)
            return;

        animator.SetBool("moving", moving);
        animator.SetBool("doJump", doJump);
        animator.SetBool("onAir", onAir);
        transform.rotation = rot;
    }
}
