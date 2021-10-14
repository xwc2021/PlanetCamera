using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public Transform moveTarget;
    public float movingTime = 3;
    public float waitTimeAtTarget=0;
    public float waitTimeAtOriginal = 0;
    public float waitTImeInit = 0;
    bool moving = false;
    float startMoveTime;
    public float debugPercentage = 0;
    Vector3 originalPos;
    Vector3 targetPos;
    Vector3 from;
    Vector3 to;
    Animator animator;
    private IEnumerator coroutine;
    // Use this for initialization
    void Awake () {

        Debug.Assert(moveTarget != null);

        originalPos = transform.position;
        targetPos = moveTarget.position;

        initAnimator();
    }

    void initAnimator()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null);

        MoveMachineInitWait state0 = animator.GetBehaviour<MoveMachineInitWait>();
        state0.movingPlatform = this;

        MoveMachineMove2Target state1 = animator.GetBehaviour<MoveMachineMove2Target>();
        state1.movingPlatform = this;

        MoveMachineWaitAtTarget state2 = animator.GetBehaviour<MoveMachineWaitAtTarget>();
        state2.movingPlatform = this;

        MoveMachineBack2Original state3 = animator.GetBehaviour<MoveMachineBack2Original>();
        state3.movingPlatform = this;

        MoveMachineWaitAtOriginal state4 = animator.GetBehaviour<MoveMachineWaitAtOriginal>();
        state4.movingPlatform = this; 
    }

    private void FixedUpdate()
    {
        if (moving)
            moveTo();
    }

    public void waitAtInit()
    {
        coroutine = waitSeconds(waitTImeInit);
        StartCoroutine(coroutine);
    }

    public void waitAtTarget()
    {
        coroutine = waitSeconds(waitTimeAtTarget);
        StartCoroutine(coroutine);
    }

    public void waitAtOriginal()
    {
        coroutine = waitSeconds(waitTimeAtOriginal);
        StartCoroutine(coroutine);
    }

    IEnumerator waitSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("okGo", true);
    }

    public void startMoveToTarget()
    {
        from = originalPos;
        to = targetPos;
        prepareMove();
    }
    public void startMoveToOriginal()
    {
        from = targetPos;
        to = originalPos;
        prepareMove();
    }

    void prepareMove()
    {
        startMoveTime = Time.fixedTime;
        moving = true;
    }

    void moveTo()
    {
        debugPercentage = (Time.fixedTime - startMoveTime)/movingTime;
        float t =Mathf.SmoothStep(0, 1, debugPercentage);
        transform.position = Vector3.Lerp(from, to,t);
;
        if (debugPercentage > 1)
        {
            animator.SetBool("reach", true);
            moving = false;
        }       
    }


}
