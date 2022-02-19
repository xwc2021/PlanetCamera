using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlanetMovable))]
public class PlanetPlayerController : MonoBehaviour
{
    SurfaceFollowHelper surfaceFollowHelper;

    public MeasuringJumpHeight measuringJumpHeight;
    PlanetMovable planetMovable;
    Rigidbody rigid;

    /* camera 相關 */
    public bool adjustCameraWhenMove = true;
    public Transform m_Cam;

    public void setCamera(Transform camera)
    {
        m_Cam = camera;
    }

    /* Animator 相關 */
    Animator animator;
    int onAirHash;

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

    /* input 相關 */
    bool doJump = false;
    public void triggerJump()
    {
        doJump = true;
    }
    public void setTurbo(bool value)
    {
        planetMovable.setTurble(value);
    }
    Vector2 moveVec;
    public Vector2 MoveVec // property
    {
        set { moveVec = value; }
    }

    /* 鎖移動方向 */
    public bool doDergeeLock = false;
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

    // Use this for initialization
    void Awake()
    {
        surfaceFollowHelper = GetComponent<SurfaceFollowHelper>();
        planetMovable = GetComponent<PlanetMovable>();
        rigid = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();
        onAirHash = Animator.StringToHash("Base Layer.onAir");
    }

    void FixedUpdate()
    {
        planetMovable.setupGravityDir();
        planetMovable.setupContactDataAndHeadUp();

        planetMovable.executeGravityForce();
        planetMovable.executeMoving(getMoveForceFromInput());

        setAnimatorMoving();

        processWallJump();
        processJump();
        processLadding();

        // 從Update移到FixedUpdate
        // 因為無法保證FixedUpdate在第1個frame一定會執行到
        if (surfaceFollowHelper != null)
            surfaceFollowHelper.doAdjustByGroundUp();
    }

    /* 行為相關 */
    Vector3 getMoveForceFromInput()
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

            var playerHeadUp = transform.up;
            Vector3 moveForword = Vector3.Cross(m_Cam.right, playerHeadUp).normalized;
            Vector3 controllForce = h * m_Cam.right + v * moveForword;
            return controllForce.normalized;
        }

        return Vector3.zero;
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
            // Marking mark green
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
}