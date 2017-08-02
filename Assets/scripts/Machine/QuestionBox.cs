using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBox : MonoBehaviour {

    bool doOnce = false;
    bool useTimer = false;
    float nowTime = 0;
    public float usefulTime = 0.25f;
    public Animator originalBoxAnimator;
    public MeshRenderer originalBox;
    public MeshRenderer nullBox;
    QBoxReady qBoxReady;
    QBoxFinal qBoxFinal;
    QBoxDoScale qBoxDoScale;

    void initAnimator()
    {
        qBoxReady = originalBoxAnimator.GetBehaviour<QBoxReady>();
        qBoxFinal = originalBoxAnimator.GetBehaviour<QBoxFinal>();
        qBoxDoScale = originalBoxAnimator.GetBehaviour<QBoxDoScale>();
        Debug.Assert(qBoxReady != null);
        Debug.Assert(qBoxFinal != null);
        Debug.Assert(qBoxDoScale != null);
        qBoxReady.qBox = this;
        qBoxFinal.qBox = this;
        qBoxDoScale.qBox = this;
    }

    // Use this for initialization
    void Start() {
        initAnimator();
    }

    void OnTriggerEnter(Collider other)
    { 
        if (other.tag != TagDefined.Player)
            return;

        originalBoxAnimator.SetBool("doScale", true);
        useTimer = true;
    }

    private void Update()
    {
        if (useTimer)
            nowTime = nowTime + Time.deltaTime;
    }

    public bool timeIsOver()
    {
        if (useTimer && doOnce)
            return nowTime >= usefulTime;
        else
            return false;
    }

    public void doFinalAction()
    {
        originalBox.enabled = false;
        nullBox.enabled = true;
    }

    public void hitBox()
    {
        doOnce = true;
    }

}
