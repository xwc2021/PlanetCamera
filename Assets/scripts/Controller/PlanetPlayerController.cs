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

    public MeasuringJumpHeight measuringJumpHeight;
    PlanetMovable planetMovable;

    public bool adjustCameraWhenMove = true;
    public Transform m_Cam;
    Rigidbody rigid;
    Animator animator;
    int onAirHash;

    /* input 相關*/
    bool doJump = false;
    bool doTurble = false;
    public bool DoTurbe
    {
        set { doTurble = value; }
    }
    Vector2 moveVec;
    public Vector2 MoveVec // property
    {
        set { moveVec = value; }
    }

    bool MoveController.doTurbo()
    {
        return doTurble;
    }

    // Use this for initialization
    void Awake()
    {
        planetMovable = GetComponent<PlanetMovable>();
        surfaceFollowHelper = GetComponent<SurfaceFollowHelper>();

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
        else
        {
            // 在空中跳，就取消
            doJump = false;
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

    public void triggerJump()
    {
        doJump = true;
    }

    void doDegreeLock(ref float h, ref float v)
    {
        //16個方向移動
        int lockPiece = 16;
        float snapDegree = 360.0f / lockPiece;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(v, h);

        float extraDegree = degree % snapDegree;
        float extraRad = extraDegree * Mathf.Deg2Rad;

        // 作旋轉修正(這是複數乘法，複數相乘=>角度相加，長度相乘)
        float newH = h * Mathf.Cos(-extraRad) + v * -Mathf.Sin(-extraRad);
        float newV = h * Mathf.Sin(-extraRad) + v * Mathf.Cos(-extraRad);

        h = newH;
        v = newV;
    }

    public bool doDergeeLock = false;
    Vector3 MoveController.getMoveForce()
    {
        Vector2 hv = moveVec;
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