using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FactoryPlugin
{
    void doIt(GameObject gameObject);
}

public class CreatePlayer : MonoBehaviour
{
    public MonoBehaviour[] factoryPloginSocket;
    public GameObject source;

    public Transform dummy;
    public CameraPivot cameraPivotPrefab;
    public MoveForceParameterRepository moveForceParameterRepositoryPrefab;
    // Use this for initialization
    void Awake()
    {
        // 生成玩家需要的所有元件
        var gameObject = GameObject.Instantiate<GameObject>(source, transform.position, transform.rotation);

        var followTarget = GameObject.Instantiate<Transform>(dummy);
        followTarget.name = "followTarget";
        followTarget.parent = gameObject.transform;
        followTarget.localPosition = new Vector3(0, 0, -1.45f);

        var cameraTarget = GameObject.Instantiate<Transform>(dummy);
        cameraTarget.name = "cameraTarget";
        cameraTarget.parent = gameObject.transform;
        cameraTarget.localPosition = new Vector3(0, 1, 0);

        var cameraPivot = GameObject.Instantiate<CameraPivot>(cameraPivotPrefab);
        cameraPivot.bind(cameraTarget, gameObject.transform);

        var moveForceParameterRepository = GameObject.Instantiate<MoveForceParameterRepository>(moveForceParameterRepositoryPrefab);
        moveForceParameterRepository.transform.parent = gameObject.transform;
        moveForceParameterRepository.transform.localPosition = Vector3.zero;

        gameObject.AddComponent<SetCameraPivot>();
        var measuringJumpHeight = gameObject.AddComponent<MeasuringJumpHeight>();

        var slopeForceMonitor = gameObject.AddComponent<SlopeForceMonitor>();
        slopeForceMonitor.maxForceLimit = 120;

        var planetMovable = gameObject.AddComponent<PlanetMovable>();
        planetMovable.slopeForceMonitor = slopeForceMonitor;
        planetMovable.moveForceParameterRepository = moveForceParameterRepository;

        var surfaceFollowHelper = gameObject.AddComponent<SurfaceFollowHelper>();
        surfaceFollowHelper.cameraPivot = cameraPivot;

        var planetPlayerController = gameObject.AddComponent<PlanetPlayerController>();
        planetPlayerController.measuringJumpHeight = measuringJumpHeight;
        planetPlayerController.setCamera(cameraPivot.getCameraTransform());

        planetMovable.init(planetPlayerController);

        // plugin 的部分
        int pluginSide = factoryPloginSocket.Length;
        for (int i = 0; i < pluginSide; i++)
        {
            FactoryPlugin fg = factoryPloginSocket[i] as FactoryPlugin;
            if (fg != null)
                fg.doIt(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
