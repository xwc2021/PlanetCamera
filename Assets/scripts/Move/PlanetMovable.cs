using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlanetMovable : MonoBehaviour
{
    static float debugLen = 2;
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

        if (moveForceParameterRepository)
            moveForceParameterRepository.resetGroundType(GroundType.Normal, rigid);

        contactPointGround = new List<ContactPoint>();
        contactPointWall = new List<ContactPoint>();

        rigid.interpolation = RigidbodyInterpolation.None;
    }

    /* 接觸相關 */
    bool doContactDetect;
    public bool Ladding
    {
        // get { return contactGround; }
        get { return contactGround || (heightToFloor < 0.1f); }
    }

    Vector3 getGroundOrFloorNormal()
    {
        if (contactGround)
            return contactGroundNormal;
        else
            return hitFloorNormal;
    }

    static float max_cos_value = Mathf.Cos(80 * Mathf.Deg2Rad);
    static float min_cos_value = Mathf.Cos(100 * Mathf.Deg2Rad);
    bool isWallNormal(Vector3 normal)
    {
        var dotValue = Vector3.Dot(upDir, normal);
        return (dotValue > min_cos_value && dotValue < max_cos_value);
    }

    /* 接觸相關 */

    // https://docs.unity3d.com/Manual/ExecutionOrder.html
    // 執行順序
    // FixedUpdate()：呼叫setupUpForPawn()
    // OnCollisionStay()：先收集contanct資訊並分類
    // Update()：判定有沒有接觸

    // 放contact資訊
    List<ContactPoint> contactPointGround;
    List<ContactPoint> contactPointWall;

    // 收集contanct資訊並分類(有可能同時碰到2個以上的物件)
    // rigid on collider | rigid on rigid
    // kinematic rigidbody 不行
    void OnCollisionStay(Collision collision)
    {
        if (!doContactDetect)
            return;

        var layer = collision.gameObject.layer;
        if (layer == LayerDefined.Border || layer == LayerDefined.BorderBlockCamera || layer == LayerDefined.BorderNoAvoid)
        {
            var len = collision.contactCount;
            for (var i = 0; i < len; ++i)
            {
                var cp = collision.GetContact(i);
                if (isWallNormal(cp.normal))
                {
                    if (layer != LayerDefined.BorderNoAvoid)
                        contactPointWall.Add(cp);
                }
                else
                    contactPointGround.Add(cp);
            }
        }
    }

    private void Update()
    {
        if (!doContactDetect)
            return;

        // 判定有沒有接觸
        contactGround = hasContact(contactPointGround, ref contactGroundNormal, Color.white);
        touchWall = hasContact(contactPointWall, ref touchWallNormal, Color.cyan);
    }

    bool hasContact(List<ContactPoint> list, ref Vector3 contactNormal, Color color)
    {
        int listCount = list.Count;
        bool contact = false;
        // 想看所有的normal
        for (int x = 0; x < listCount; x++)
        {
            var cp = list[x];

            if (color != Color.black)
                Debug.DrawRay(cp.point, cp.normal * debugLen, color);

            contactNormal = cp.normal;
            contact = true;
        }
        return contact;
    }

    bool contactGround;
    Vector3 contactGroundNormal;

    bool touchWall;
    public bool TouchWall { get { return touchWall; } }

    Vector3 touchWallNormal;
    public Vector3 TouchWallNormal
    {
        get { return touchWallNormal; }
    }

    /* 接觸相關：(跳上摩天輪和電纜需要，因為 它們的parent有 kinematic rigidbody) */
    public float heightToFloor;
    bool isHitFloor = false;
    Vector3 hitFloorNormal;
    Vector3 hitFloorPos;
    public Vector3 HitFloorPos
    {
        get => hitFloorPos;
    }
    void hitFloor()
    {
        float rayCastDistance = 5;
        float rayFromUpOffset = 1;

        Vector3 from = transform.position + upDir * rayFromUpOffset;
        Debug.DrawRay(from, -upDir * rayCastDistance, Color.yellow);

        heightToFloor = float.MaxValue;
        isHitFloor = false;

        RaycastHit hit;
        int layerMask = 1 << LayerDefined.HitFloor;
        if (Physics.Raycast(from, -upDir, out hit, rayCastDistance, layerMask))
        {
            heightToFloor = (hit.point - from).magnitude - rayFromUpOffset;
            hitFloorNormal = hit.normal;
            hitFloorPos = hit.point;
            isHitFloor = true;
            Debug.DrawRay(hit.point, hitFloorNormal * debugLen, Color.black);
        }
    }

    /* 移動相關：準備工具 called in FixedUpdate */
    public void preProcess(bool doContactDetect, bool standUp)
    {
        setupGravityDir();

        this.doContactDetect = doContactDetect;
        if (doContactDetect)
            setupUpForContact();

        if (standUp)
        {
            // 設定面向
            Vector3 forward = Vector3.Cross(transform.right, upDir);
            Quaternion targetRotation = Quaternion.LookRotation(forward, upDir);

            if (Ladding)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime);
            else transform.rotation = targetRotation;
        }
    }

    static Color orange = new Color(229.0f / 255, 83.0f / 255, 0);
    void setupGravityDir()
    {
        // 重力朝向:預設向下
        var pos = transform.position;
        gravityDir = this.grounGravityGenerator != null ? this.grounGravityGenerator.findGravityDir(transform.up, pos, isHitFloor, hitFloorPos) : -Vector3.up;
        upDir = -gravityDir;

        Debug.DrawRay(pos, upDir * debugLen * 2, orange);
    }

    void setupUpForContact()
    {
        // 準備 (rigid on collider)
        contactPointGround.Clear();
        contactPointWall.Clear();

        // 擊地板 (rigid on rigid)
        hitFloor();
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

    public void executeGravityForce()
    {
        if (moveForceParameterRepository == null)
        {
            rigid.AddForce(10 * gravityDir, ForceMode.Acceleration);
            return;
        }

        // 如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getGravityForceStrength(!Ladding) * gravityDir, ForceMode.Acceleration);

        // Debug.DrawRay(transform.position, gravityDir, Color.green);

        // 三軸
        // Debug.DrawRay(transform.position, transform.up * 2, Color.green);
        // Debug.DrawRay(transform.position, transform.forward, Color.blue);
        // Debug.DrawRay(transform.position, transform.right, Color.red);
    }

    public void executeMoving(Vector3 moveForce)
    {
        // Debug.DrawRay(transform.position, moveForce * debugLen * 2, Color.blue);
        if (moveForce == Vector3.zero)
            return;

        // 滑過障礙物
        avoidObstacle(ref moveForce);

        // 貼著地板移動
        if (Ladding)
        {
            moveForce = Vector3.ProjectOnPlane(moveForce, getGroundOrFloorNormal());
            moveForce.Normalize();
            Debug.DrawRay(transform.position + transform.up, moveForce * debugLen, Color.blue);
        }

        // 更新面向
        if (moveForce != Vector3.zero && !firstPersonMode)
        {
            Quaternion targetRotation2 = Quaternion.LookRotation(moveForce, upDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.fixedDeltaTime * rotationSpeed);
        }

        // 取得移動的力
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        float moveForceStrength = moveForceParameter.getMoveForceStrength(!Ladding, isTurble);
        Vector3 moveForceWithStrength = moveForceStrength * moveForce;

        // 處理斜坡
        if (slopeForceMonitor != null && Ladding)
            moveForceWithStrength = slopeForceMonitor.modifyMoveForce(moveForceWithStrength, moveForceParameter.getGravityForceStrength(!Ladding), upDir, getGroundOrFloorNormal());

        // 移動
        rigid.AddForce(moveForceWithStrength, ForceMode.Acceleration);
        // print(rigid.velocity.magnitude);
    }

    public void executeJump()
    {
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getJumpForceStrength(isTurble) * -gravityDir, ForceMode.Acceleration);
    }

    public bool avoidObstacleQuickMethod = true;
    /* 滑過障礙物相關 */
    public void avoidObstacle(ref Vector3 moveForce)
    {
        bool isGet;
        Vector3 obstacleNormal;
        if (avoidObstacleQuickMethod)
            getObstacleNormalFromTouchWallNormal(out obstacleNormal, out isGet);
        else
            getObstacleNormalFromSphereCast(out obstacleNormal, out isGet);

        if (!isGet)
            return;

        // 投影到移動平面
        obstacleNormal = Vector3.ProjectOnPlane(touchWallNormal, upDir);
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

    // 直接拿touchWall的資料
    void getObstacleNormalFromTouchWallNormal(out Vector3 obstacleNormal, out bool isGet)
    {

        isGet = touchWall;
        obstacleNormal = touchWallNormal;
    }

    // 比較吃效能，但移動起來更加絲滑
    void getObstacleNormalFromSphereCast(out Vector3 obstacleNormal, out bool isGet)
    {
        isGet = false;
        obstacleNormal = Vector3.zero;

        int layerMask = 1 << LayerDefined.Border | 1 << LayerDefined.BorderBlockCamera;
        RaycastHit hit;

        float SphereR = 0.24f;
        float forwardToWall = 0.5f;
        float leftRightTowall = 0.2f;
        float[] rayCastDistanceToWall = { forwardToWall, forwardToWall, forwardToWall, leftRightTowall, leftRightTowall };
        Vector3[] dir = { transform.forward, transform.forward, transform.forward, transform.right, -transform.right };
        float[] height = { 0.25f, 0.6f, 1.2f, 0.6f, 0.6f };

        for (int i = 0; i < 5; i++)
        {
            Vector3 from = upDir * height[i] + transform.position;
            Debug.DrawRay(from, dir[i] * rayCastDistanceToWall[i], Color.green);
            if (Physics.SphereCast(from, SphereR, dir[i], out hit, rayCastDistanceToWall[i], layerMask) && isWallNormal(hit.normal))
            {
                isGet = true;
                obstacleNormal = hit.normal;
                Debug.DrawRay(hit.point, hit.normal * debugLen, Color.yellow);
                return;
            }
        }
    }
}