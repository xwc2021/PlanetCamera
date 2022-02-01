using UnityEngine;

public interface MoveController
{
    Vector3 getMoveForce();
    bool doTurbo();
}

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlanetMovable))]
public class PlanetPlayerController : MonoBehaviour, MoveController
{
    SurfaceFollowHelper surfaceFollowHelper;
    MultiplayerCameraManager multiplayerCameraManager;
    public GameObject canvas;
    public GameObject eventSystem;
    public MeasuringJumpHeight measuringJumpHeight;
    PlanetMovable planetMovable;

    public bool adjustCameraWhenMove = true;
    public Transform m_Cam;
    Rigidbody rigid;
    Animator animator;
    int onAirHash;

    bool doJump = false;
    // Use this for initialization
    void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        surfaceFollowHelper = GetComponent<SurfaceFollowHelper>();
        multiplayerCameraManager = GetComponent<MultiplayerCameraManager>();

        //print("cameraBehavior="+cameraBehavior);
        var inputProxy = InputManager.getInputProxy();
        if (inputProxy.enableControlUI())
        {
            canvas.SetActive(true);
            eventSystem.SetActive(true);
        }

        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        onAirHash = Animator.StringToHash("Base Layer.onAir");

    }

    public void setCamera(Transform camera)
    {
        m_Cam = camera;
    }

    void setAnimatorMoving()
    {
        bool moving = rigid.velocity.magnitude > 0.05;
        animator.SetBool("moving", moving);
    }

    void setAnimatorDoJump()
    {
        animator.SetBool("doJump", true);
    }

    void setAnimatorOnAir()
    {
        animator.SetBool("onAir", false);
    }

    void syncAnimatorAndRot()
    {
        if (multiplayerCameraManager != null)
        {
            bool moving = animator.GetBool("moving");
            bool doJump = animator.GetBool("doJump");
            bool onAir = animator.GetBool("onAir");
            multiplayerCameraManager.CmdSyncAnimatorAndRot(moving, doJump, onAir, transform.rotation);
        }
    }

    void FixedUpdate()
    {
        planetMovable.setupGravity();
        planetMovable.setupRequireData();

        syncPositionByPlatform();

        planetMovable.executeGravityForce();
        planetMovable.executeMoving();

        setAnimatorMoving();

        processWallJump();
        processJump();
        processLadding();

        syncAnimatorAndRot();

        //從Update移到FixedUpdate
        //因為無法保證FixedUpdate在第1個frame一定會執行到
        if (surfaceFollowHelper != null)
            surfaceFollowHelper.doAdjustByGroundUp();
    }



    void processWallJump()
    {
        //jump from wall
        if (!planetMovable.Ladding && planetMovable.TouchWall)
        {
            if (doJump)
            {
                planetMovable.executeJump();
                float s = 20;
                rigid.AddForce(s * planetMovable.TouchWallNormal, ForceMode.VelocityChange);
                doJump = false;
            }
        }
    }

    void processJump()
    {
        //跳
        if (planetMovable.Ladding)
        {
            Debug.DrawLine(transform.position, transform.position - transform.up, Color.green);
            if (doJump)
            {
                if (measuringJumpHeight != null)
                    measuringJumpHeight.startRecord();

                planetMovable.executeJump();

                setAnimatorDoJump();

                doJump = false;
            }
        }
    }

    void processLadding()
    {
        if (planetMovable.Ladding)
        {
            if (animator != null)
            {
                bool isOnAir = onAirHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (isOnAir)
                {
                    setAnimatorOnAir();
                    if (measuringJumpHeight != null)
                        measuringJumpHeight.stopRecord();
                }
            }
        }
    }


    void Update()
    {
        //這邊要加上if (!doJump)的判斷，因為：
        // 如果在|frame1|GetButtonDown為true，但在|frame1|沒有執行FixedUpdate
        // 這樣到了|frame2|GetButtonDown為false，就不會跳了

        if (!doJump)
            doJump = InputManager.getInputProxy().pressJump();
    }

    //https://msdn.microsoft.com/zh-tw/library/14akc2c7.aspx
    void doDegreeLock(ref float h, ref float v)
    {
        //16個方向移動
        int lockPiece = 16;
        float snapDegree = 360.0f / lockPiece;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(v, h);

        float extraDegree = degree % snapDegree;

        float extraRad = extraDegree * Mathf.Deg2Rad;

        //作旋轉修正
        float newH = h * Mathf.Cos(-extraRad) + v * -Mathf.Sin(-extraRad);
        float newV = h * Mathf.Sin(-extraRad) + v * Mathf.Cos(-extraRad);

        h = newH;
        v = newV;
    }

    public bool doDergeeLock = false;
    Vector3 MoveController.getMoveForce()
    {
        //取得輸入
        Vector2 hv = InputManager.getInputProxy().getHV();
        hv.Normalize();
        float h = hv.x;
        float v = hv.y;

        if (h != 0 || v != 0)
        {
            if (m_Cam == null)
                return Vector3.zero;

            if (doDergeeLock)
                doDegreeLock(ref h, ref v);

            Vector3 moveForword = Vector3.Cross(m_Cam.right, transform.up);
            Vector3 controllForce = h * m_Cam.right + v * moveForword;
            return controllForce.normalized;
        }

        return Vector3.zero;
    }

    bool MoveController.doTurbo()
    {
        return InputManager.getInputProxy().holdFire();
    }

    RecordPositionDiff platform;
    public void setPlatform(RecordPositionDiff pPlatform)
    {
        platform = pPlatform;
    }

    public void clearPlatform()
    {
        platform = null;
    }

    //when using "set parent method" to move player on MovingPlatform.
    //this method will not be called.

    void syncPositionByPlatform()
    {
        if (platform == null)
            return;

        rigid.position += platform.getDiff();
        //transform.position += platform.getDiff();
    }
}