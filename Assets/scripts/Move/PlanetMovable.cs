using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlanetMovable : MonoBehaviour
{
    static float debugLen = 5;
    /* 重力相關 */
    Vector3 gravityDir;
    GroundGravityGenerator grounGravityGenerator;
    public void ResetGravityGenetrator(GroundGravityGenerator gg)
    {
        this.grounGravityGenerator = gg;
    }
    Vector3 upDir; // 重力的反方向
    public Vector3 UpDir { get { return upDir; } }
    public SlopeForceMonitor slopeForceMonitor; // 計算重力斜坡分量

    public void init()
    {
        rigid = GetComponent<Rigidbody>();
        Debug.Assert(moveForceParameterRepository != null);
        moveForceParameterRepository.resetGroundType(GroundType.Normal, rigid);

        contactPointGround = new List<ContactPoint[]>();
        contactPointWall = new List<ContactPoint[]>();

        rigid.interpolation = RigidbodyInterpolation.None;
    }

    /* 接觸相關 */

    // 執行順序
    // https://docs.unity3d.com/Manual/ExecutionOrder.html
    // FixedUpdate()
    // void OnCollisionStay(Collision collision)
    // Update()
    List<ContactPoint[]> contactPointGround;
    List<ContactPoint[]> contactPointWall;
    void OnCollisionStay(Collision collision)
    {
        bool ground = collision.gameObject.layer == LayerDefined.ground;
        bool groundNotBlockCamera = collision.gameObject.layer == LayerDefined.groundNotBlockCamera;

        if (ground || groundNotBlockCamera)
        {
            //有可能同時碰到2個以上的物件，所以先收集起來
            contactPointGround.Add(collision.contacts);
        }

        bool wall = collision.gameObject.layer == LayerDefined.wall;
        bool wallNotBlockCamera = collision.gameObject.layer == LayerDefined.wallNotBlockCamera;
        if (wall | wallNotBlockCamera)
        {
            //有可能同時碰到2個以上的物件，所以先收集起來
            contactPointWall.Add(collision.contacts);
        }
    }
    private void Update()
    {
        // 判定有沒有接觸
        contactGround = isContactGround();
        touchWall = isTouchWall();
    }

    public bool Ladding { get { return contactGround; } }
    bool contactGround;
    Vector3 contactGroundNormal;
    bool isContactGround()
    {
        int listCount = contactPointGround.Count;
        bool contact = false;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointGround[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Debug.DrawRay(cp[i].point, cp[i].normal * debugLen);
                contactGroundNormal = cp[i].normal;
                contact = true;
            }
        }
        return contact;
    }

    bool touchWall;
    public bool TouchWall { get { return touchWall; } }

    Vector3 touchWallNormal;
    public Vector3 TouchWallNormal
    {
        get { return touchWallNormal; }
    }
    bool isTouchWall()
    {
        int listCount = contactPointWall.Count;
        bool touch = false;
        // 這裡是因為想看所有的contact Normal，不然不需要跑迴圈
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointWall[x];
            for (int i = 0; i < cp.Length; i++)
            {
                touchWallNormal = cp[i].normal;
                Debug.DrawRay(cp[i].point, cp[i].normal * debugLen);
                touch = true;
            }
        }
        return touch;
    }

    public float heightToFloor;
    void hitFloor()
    {
        float rayCastDistance = 5;
        float rayFromUpOffset = 1;
        float rayFromForwardOffset = -0.1f; //往後退一步，下斜坡不卡住(因為在交界處有如果直直往下打可能打中斜坡)

        Vector3 from = transform.position + upDir * rayFromUpOffset + transform.forward * rayFromForwardOffset;
        Debug.DrawRay(from, -upDir * rayCastDistance, Color.yellow);

        RaycastHit hit;
        heightToFloor = float.PositiveInfinity;
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.groundNotBlockCamera;
        if (Physics.Raycast(from, -upDir, out hit, rayCastDistance, layerMask))
            heightToFloor = (hit.point - from).magnitude - rayFromUpOffset;
    }

    /* 移動相關 called in FixedUpdate */
    Rigidbody rigid;
    public MoveForceParameterRepository moveForceParameterRepository;
    public void resetGroundType(GroundType groundType)
    {
        moveForceParameterRepository.resetGroundType(groundType, rigid);
    }
    public float rotationSpeed = 6f;
    public bool firstPersonMode = false;
    bool isTurble = false;
    public void setTurble(bool value) { isTurble = value; }
    public void setupGravityDir()
    {
        // 重力朝向:預設向下
        var pos = transform.position;
        gravityDir = this.grounGravityGenerator != null ? this.grounGravityGenerator.findGravityDir(transform.up, ref pos) : -Vector3.up;
        upDir = -gravityDir;
    }

    public void setupContactData()
    {
        // 清空
        contactPointGround.Clear();
        contactPointWall.Clear();

        // 設定面向
        Vector3 forward = Vector3.Cross(transform.right, upDir);
        Quaternion targetRotation = Quaternion.LookRotation(forward, upDir);
        transform.rotation = targetRotation;

        // 擊中地板
        hitFloor();
        var isTouchFloor = heightToFloor < 0.1f;
    }

    public void executeGravityForce()
    {
        // 如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getGravityForceStrength(!Ladding) * gravityDir, ForceMode.Acceleration);
        // Debug.DrawRay(transform.position, gravityDir, Color.green);
    }

    public void executeMoving(Vector3 moveForce)
    {
        // Debug.DrawRay(transform.position, moveForce * debugLen * 2, Color.blue);
        if (moveForce == Vector3.zero)
            return;

        // 滑過障礙物
        modifyMoveForceAlongObstacle(ref moveForce);

        // 貼著地板移動
        if (Ladding)
        {
            moveForce = Vector3.ProjectOnPlane(moveForce, contactGroundNormal);
            moveForce.Normalize();
            Debug.DrawRay(transform.position + transform.up, moveForce * debugLen, Color.blue);
        }

        // 更新面向
        if (moveForce != Vector3.zero && !firstPersonMode)
        {
            Quaternion targetRotation2 = Quaternion.LookRotation(moveForce, upDir);
            Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
            transform.rotation = newRot;
        }

        // 取得移動的力
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        float moveForceStrength = moveForceParameter.getMoveForceStrength(!Ladding, isTurble);
        Vector3 moveForceWithStrength = moveForceStrength * moveForce;

        // 處理斜坡
        if (slopeForceMonitor != null && Ladding)
            moveForceWithStrength = slopeForceMonitor.modifyMoveForce(moveForceWithStrength, moveForceParameter.getGravityForceStrength(!Ladding), upDir, contactGroundNormal);

        // 移動
        rigid.AddForce(moveForceWithStrength, ForceMode.Acceleration);
        // print(rigid.velocity.magnitude);
    }

    public void executeJump()
    {
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getJumpForceStrength(isTurble) * -gravityDir, ForceMode.Acceleration);
    }

    public void modifyMoveForceAlongObstacle(ref Vector3 moveForce)
    {
        // 直接拿touchWall的資料
        if (!touchWall)
            return;

        var obstacleNormal = Vector3.ProjectOnPlane(touchWallNormal, upDir);
        Debug.DrawRay(transform.position, obstacleNormal * debugLen, Color.red);

        // 離開牆不用處理
        if (Vector3.Dot(moveForce.normalized, obstacleNormal) > 0)
            return;

        // 消去和wallNormal垂直的分量
        Vector3 modifyMoveForce = Vector3.ProjectOnPlane(moveForce, obstacleNormal);

        // 如果移動的方向和wallNormal接近垂直，newMoveForce就可能變的很短
        if (modifyMoveForce.magnitude < 0.01f)
            return;

        moveForce = modifyMoveForce;
        // print("修改");
    }
}