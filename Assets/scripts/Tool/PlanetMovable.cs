//#define FollowCameraInLateUpdate

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public interface InputProxy
{
    Vector2 getHV();
    bool pressJump();
    bool pressFire();
    bool holdFire();
    float yawScale();
    float pitchScale();
    bool enableControlUI();
}

public interface GroundGravityGenerator
{
    Vector3 findGroundUp();
}

public interface MoveForceParameter
{
    float getMoveForceStrength(bool isOnAir, bool isTurble);
    float getGravityForceStrength(bool isOnAir);
    float getJumpForceStrength(bool isTurble);
    void setRigidbodyParamter(Rigidbody rigid);
}


[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlanetMovable : MonoBehaviour
{
    public SlopeForceMonitor slopeForceMonitor;

    public GravityDirectionMonitor gravityDirectionMonitor;
    public MoveForceParameterRepository moveForceParameterRepository;

    public MoveController moveController;

    Rigidbody rigid;
    public float rotationSpeed = 6f;
    public bool firstPersonMode = false;
    public float backOffset = -0.1f;

    
    

    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    List<ContactPoint[]> contactPointGround;
    List<ContactPoint[]> contactPointWall;
    bool contactGround;
    bool touchWall;
    bool isHit = false;
    bool ladding = false;
    bool isTurble=false;

    static readonly float isHitDistance = 0.2f;
    public static readonly float rayCastDistanceToGround = 2;
    Vector3 groundUp;
    Vector3 gravityDir;
    Vector3 planeNormal;
    Vector3 wallNormal;
  

    // Use this for initialization
    void Awake () {

        rigid = GetComponent<Rigidbody>();
        Debug.Assert(moveForceParameterRepository != null);
        moveForceParameterRepository.resetGroundType(GroundType.Normal, rigid);

        contactPointGround = new List<ContactPoint[]>();
        contactPointWall= new List<ContactPoint[]>();

        moveController = GetComponent<MoveController>();
        Debug.Assert(moveController != null);

#if (FollowCameraInLateUpdate)

    rigid.interpolation = RigidbodyInterpolation.Interpolate;
#else
    rigid.interpolation = RigidbodyInterpolation.None;   
#endif
    }

    public void ResetGravityGenetrator(GravityGeneratorEnum pggEnum)
    {
        gravityDirectionMonitor.ResetGravityGenerator(pggEnum);
    }

    private void Update()
    {
        //判定有沒有接觸
        contactGround = isContactGround();
        touchWall =isTouchWall(); 
    }

    private void getGroundNormalNow(out bool isHit)
    {
        RaycastHit hit;
        //https://www.youtube.com/watch?v=Cq_Wh8o96sc
        //往後退一步，下斜坡不卡住(因為在交界處有如果直直往下打可能打中斜坡)
        Vector3 from = transform.forward * backOffset + groundUp + transform.position;
        //Debug.DrawRay(from, -groundUp*2 , Color.red);
        isHit = false;
        int layerMask = 1 << LayerDefined.ground | 1 << LayerDefined.groundNotBlockCamera;
        planeNormal = groundUp;
        if (Physics.Raycast(from, -groundUp, out hit, rayCastDistanceToGround, layerMask))
        {
            Vector3 diff = hit.point - transform.position;
            float height = Vector3.Dot(diff, groundUp);

            planeNormal = hit.normal;

            //如果距離小於某個值就判定是在地面上
            if (height < isHitDistance)
            {
                isHit = true;
            }   
        }
    }

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

    bool isContactGround()
    {
        int listCount = contactPointGround.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointGround[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Vector3 diif = cp[i].point - transform.position;
                float height = Vector3.Dot(groundUp, diif);
                if (height < 0.15f)
                {
                    return true;
                }
            }

        }
        return false;
    }

    Vector3 touchWallNormal;
    bool isTouchWall()
    {
        int listCount = contactPointWall.Count;
        for (int x = 0; x < listCount; x++)
        {
            ContactPoint[] cp = contactPointWall[x];
            for (int i = 0; i < cp.Length; i++)
            {
                Vector3 diif = cp[i].point - transform.position;
                float height = Vector3.Dot(groundUp, diif);
                if (height > 0.15f && height < 1.0f)
                {
                    touchWallNormal = cp[i].normal;
                    return true;
                }
            }

        }
        return false;
    }

    public void setupGravity()
    {
        //計算重力方向
        groundUp = gravityDirectionMonitor.findGroundUp();
        gravityDir = -groundUp;
    }

    public void setupRequireData()
    {
        //清空
        contactPointGround.Clear();
        contactPointWall.Clear();

        //設定面向
        Vector3 forward = Vector3.Cross(transform.right, groundUp);
        Quaternion targetRotation = Quaternion.LookRotation(forward, groundUp);
        transform.rotation = targetRotation;

        //判定是否擊中平面
        getGroundNormalNow(out isHit);

        //如果只用contact判定，下坡時可能contact為false
        ladding = contactGround || isHit;

        isTurble = moveController.doTurbo();
    }

    public void executeGravityForce()
    {
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        //如果在空中的重力加速度和在地面上時一樣，就會覺的太快落下
        rigid.AddForce(moveForceParameter.getGravityForceStrength(!ladding) * gravityDir, ForceMode.Acceleration);
        //Debug.DrawRay(transform.position, gravityDir, Color.green);
    }

    public void executeMoving()
    {
        Vector3 moveForce = moveController.getMoveForce();
        //Debug.DrawLine(transform.position, transform.position + moveForce * 10, Color.blue);

        if (moveForce == Vector3.zero)
        {
            return;
        }

        //在地面才作
        if (ladding)
        {
            moveForce = Vector3.ProjectOnPlane(moveForce, planeNormal);

            moveForce.Normalize();
            Debug.DrawRay(transform.position + transform.up, moveForce * 5, Color.blue);
        }

        //更新面向begin
        Vector3 forward2 = moveForce;
        if (forward2 != Vector3.zero && !firstPersonMode)
        {
            Quaternion targetRotation2 = Quaternion.LookRotation(forward2, groundUp);
            Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * rotationSpeed);
            transform.rotation = newRot;
        }
        //更新面向end

        //addForce可以有疊加的效果
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        float moveForceStrength = moveForceParameter.getMoveForceStrength(!ladding, isTurble);
        Vector3 moveForceWithStrength = moveForceStrength * moveForce;
        if (slopeForceMonitor != null && ladding)
        {
            moveForceWithStrength = slopeForceMonitor.modifyMoveForce(moveForce, moveForceStrength, moveForceParameter.getGravityForceStrength(!ladding), groundUp, planeNormal);
        }

        rigid.AddForce(moveForceWithStrength, ForceMode.Acceleration);
    }

    public void executeJump()
    {
        MoveForceParameter moveForceParameter = moveForceParameterRepository.getMoveForceParameter();
        rigid.AddForce(moveForceParameter.getJumpForceStrength(isTurble) * -gravityDir, ForceMode.Acceleration);
    }

    public Vector3 GravityDir
    {
        get { return gravityDir; }
    }

    public Vector3 GroundUp
    {
        get { return groundUp; }
    }

    public Vector3 TouchWallNormal
    {
        get { return touchWallNormal; }
    }

    public bool Ladding
    {
        get{return ladding;}
    }

    public bool TouchWall
    {
        get { return touchWall; }
    }

    public void resetGroundType(GroundType groundType)
    {
        moveForceParameterRepository.resetGroundType(groundType,rigid);
    }
}
