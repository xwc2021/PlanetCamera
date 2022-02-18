using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlanetMovable : MonoBehaviour
{
    Rigidbody rigid;
    public SlopeForceMonitor slopeForceMonitor;
    GroundGravityGenerator grounGravityGenerator;
    public void ResetGravityGenetrator(GroundGravityGenerator gg)
    {
        this.grounGravityGenerator = gg;
    }
    public MoveForceParameterRepository moveForceParameterRepository;
    public void resetGroundType(GroundType groundType)
    {
        moveForceParameterRepository.resetGroundType(groundType, rigid);
    }

    public void init()
    {
        rigid = GetComponent<Rigidbody>();
        Debug.Assert(moveForceParameterRepository != null);
        moveForceParameterRepository.resetGroundType(GroundType.Normal, rigid);

        contactPointGround = new List<ContactPoint[]>();
        contactPointWall = new List<ContactPoint[]>();

        rigid.interpolation = RigidbodyInterpolation.None;
    }

    bool contactGround;
    bool touchWall;
    public bool TouchWall { get { return touchWall; } }

    bool ladding = false;
    public bool Ladding
    {
        get { return ladding; }
    }
    bool isTurble = false;
    public void setTurble(bool value) { isTurble = value; }

    Vector3 upDir; // 重力的反方向
    public Vector3 UpDir { get { return upDir; } }
    Vector3 gravityDir;

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
        //判定有沒有接觸
        contactGround = isContactGround();
        touchWall = isTouchWall();
    }

    bool isContactGround()
    {
        int listCount = contactPointGround.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointGround[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Vector3 diff = cp[i].point - transform.position;
                float height = Vector3.Dot(upDir, diff);
                if (height < 0.15f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    Vector3 touchWallNormal;
    public Vector3 TouchWallNormal
    {
        get { return touchWallNormal; }
    }
    bool isTouchWall()
    {
        int listCount = contactPointWall.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointWall[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Vector3 diff = cp[i].point - transform.position;
                float height = Vector3.Dot(upDir, diff);
                if (height > 0.15f && height < 1.0f) // 玩家身高
                {
                    touchWallNormal = cp[i].normal;
                    return true;
                }
            }
        }
        return false;
    }

    /* 移動相關 called in FixedUpdate */
    public float rotationSpeed = 6f;
    public bool firstPersonMode = false;

    public void setupGravityDir()
    {
        // 重力朝向:預設向下
        var pos = transform.position;
        gravityDir = this.grounGravityGenerator != null ? this.grounGravityGenerator.findGravityDir(transform.up, ref pos) : -Vector3.up;
        upDir = -gravityDir;
    }

    static readonly float isHitDistance = 0.2f;
    static readonly float rayCastDistanceToGround = 2;
    Vector3 groundNormal;
    private void getGroundNormalNow(out bool isHitGround)
    {
        RaycastHit hit;

        //往後退一步，下斜坡不卡住(因為在交界處有如果直直往下打可能打中斜坡)
        float backOffset = -0.1f;
        Vector3 from = transform.forward * backOffset + upDir + transform.position;
        //Debug.DrawRay(from, -upDir*2 , Color.red);
        isHitGround = false;
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.groundNotBlockCamera;
        groundNormal = upDir;
        if (Physics.Raycast(from, -upDir, out hit, rayCastDistanceToGround, layerMask))
        {
            Vector3 diff = hit.point - transform.position;
            float height = Vector3.Dot(diff, upDir);

            groundNormal = hit.normal;

            //如果距離小於某個值就判定是在地面上
            if (height < isHitDistance)
            {
                isHitGround = true;
            }
        }
    }

    public void setupContactData()
    {
        //清空
        contactPointGround.Clear();
        contactPointWall.Clear();

        //設定面向
        Vector3 forward = Vector3.Cross(transform.right, upDir);
        Quaternion targetRotation = Quaternion.LookRotation(forward, upDir);
        transform.rotation = targetRotation;

        //判定是否擊中平面
        bool isHitGround;
        getGroundNormalNow(out isHitGround);

        //如果只用contact判定，下坡時可能contact為false
        ladding = contactGround || isHitGround;
    }

    public void executeGravityForce()
    {
        // 如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getGravityForceStrength(!ladding) * gravityDir, ForceMode.Acceleration);
        //Debug.DrawRay(transform.position, gravityDir, Color.green);
    }

    public void executeMoving(Vector3 moveForce)
    {
        // Debug.DrawLine(transform.position, transform.position + moveForce * 10, Color.blue);
        if (moveForce == Vector3.zero)
            return;

        // 滑過障礙物
        modifyMoveForceAlongObstacle(ref moveForce);

        // 在地面才作
        if (ladding)
        {
            moveForce = Vector3.ProjectOnPlane(moveForce, groundNormal);

            moveForce.Normalize();
            Debug.DrawRay(transform.position + transform.up, moveForce * 5, Color.blue);
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
        float moveForceStrength = moveForceParameter.getMoveForceStrength(!ladding, isTurble);
        Vector3 moveForceWithStrength = moveForceStrength * moveForce;

        // 處理斜坡
        if (slopeForceMonitor != null && ladding)
            moveForceWithStrength = slopeForceMonitor.modifyMoveForce(moveForceWithStrength, moveForceParameter.getGravityForceStrength(!ladding), upDir, groundNormal);

        // 移動
        rigid.AddForce(moveForceWithStrength, ForceMode.Acceleration);
        // print(rigid.velocity.magnitude);
    }

    public void executeJump()
    {
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getJumpForceStrength(isTurble) * -gravityDir, ForceMode.Acceleration);
    }

    /* 避免卡障礙物相關 */
    void getHitWallNormal(out Vector3 wallNormal, out bool isHitWall)
    {
        int layerMask = 1 << LayerDefined.wall | 1 << LayerDefined.wallNotBlockCamera;
        RaycastHit hit;

        isHitWall = false;
        wallNormal = Vector3.zero;

        float SphereR = 0.24f;
        float forwardToWall = 0.5f;
        float leftRightTowall = 0.2f;
        float[] rayCastDistanceToWall = { forwardToWall, forwardToWall, forwardToWall, leftRightTowall, leftRightTowall };


        float heightThreshold = 0.1f;
        float[] height = { 0.25f, 0.6f, 1.2f, 0.6f, 0.6f };
        Vector3[] dir = { transform.forward, transform.forward, transform.forward, transform.right, -transform.right };
        for (int i = 0; i < 5; i++)
        {
            Vector3 from = upDir * height[i] + transform.position;
            Debug.DrawRay(from, dir[i] * rayCastDistanceToWall[i], Color.green);
            if (Physics.SphereCast(from, SphereR, dir[i], out hit, rayCastDistanceToWall[i], layerMask))
            {
                float h = Vector3.Dot(hit.point - transform.position, upDir);

                //高過才算
                if (h > heightThreshold)
                {
                    isHitWall = true;
                    wallNormal = hit.normal;
                    Debug.DrawRay(hit.point, hit.normal * 2, Color.red);
                    Debug.DrawRay(from, transform.forward, Color.yellow);
                    return;
                }
            }
        }
    }

    public void modifyMoveForceAlongObstacle(ref Vector3 moveForce)
    {
        bool isHit;
        Vector3 wallNormal;
        getHitWallNormal(out wallNormal, out isHit);

        if (!isHit)
            return;

        wallNormal = Vector3.ProjectOnPlane(wallNormal, upDir);
        Debug.DrawRay(transform.position, wallNormal, Color.red);
        float dotValue = Vector3.Dot(moveForce, wallNormal);

        // 離開牆不用管
        if (Vector3.Dot(moveForce.normalized, wallNormal) > 0)
            return;

        // 消去和wallNormal垂直的分量
        Vector3 newMoveForce = Vector3.ProjectOnPlane(moveForce, wallNormal);

        // 如果移動的方向和wallNormal接近垂直，newMoveForce就可能變的很短
        if (newMoveForce.magnitude < 0.01f)
            return;

        moveForce = newMoveForce;
        // print("修改");
    }
}