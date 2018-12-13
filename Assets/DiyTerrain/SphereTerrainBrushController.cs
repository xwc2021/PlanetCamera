using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum StitchingCase
{
    Top_Forward,
    Top_Right,
    Top_Back,
    Top_Left,

    Back_Right,
    Right_Forward,
    Forward_Left,
    Left_Back,

    Bottom_Forward,
    Bottom_Right,
    Bottom_Back,
    Bottom_Left,
}

public class SphereTerrainBrushController : MonoBehaviour
{
    public float brushStrength = 0.1f;
    public float brushSize = 10.0f;
    public bool clear = false;
    void Update()
    {
        foreach (var s in sphereTerrains)
        {
            s.updateBrushStrength(brushStrength);
            s.updateBrushSzie(brushSize);

            s.clearHeight(clear);
        }
    }

    public void doStitch()
    {
        StartCoroutine(stitching());
    }

    public IEnumerator stitching()
    {
        stitch12(true);
        yield return new WaitForEndOfFrame();
        stitch12(false);
    }

    void stitch12(bool value)
    {
        var topTerrain = this.sphereTerrains[0];
        var bottomTerrain = this.sphereTerrains[1];
        var backTerrain = this.sphereTerrains[2];
        var rightTerrain = this.sphereTerrains[3];
        var forwardTerrain = this.sphereTerrains[4];
        var leftTerrain = this.sphereTerrains[5];

        // 上4
        // StitchingCase.Top_Forward:
        topTerrain.stitchingUp.gameObject.SetActive(value);
        forwardTerrain.stitchingDown.gameObject.SetActive(value);

        // case StitchingCase.Top_Right:
        topTerrain.stitchingRight.gameObject.SetActive(value);
        rightTerrain.stitchingUp.gameObject.SetActive(value);

        // case StitchingCase.Top_Back:


        // case StitchingCase.Top_Left:


        // 中4
        // case StitchingCase.Back_Right:


        // case StitchingCase.Right_Forward:


        // case StitchingCase.Forward_Left:


        // case StitchingCase.Left_Back:



        //下4
        // case StitchingCase.Bottom_Forward:


        // case StitchingCase.Bottom_Right:


        // case StitchingCase.Bottom_Back:


        // case StitchingCase.Bottom_Left:
    }

    DrawHeightCamera[] drawHeightCameras;
    public void saveTexture()
    {
        foreach (var drawHeightCamera in drawHeightCameras)
            drawHeightCamera.DumpRenderTexture(Application.dataPath + "/savePng/");
    }

    public Transform from;
    public Transform to;
    public Transform hitOnPlane;
    public Transform rotAlongBorder;

    public static SphereTerrainBrushController instance;

    void Start()
    {
        instance = this;
        drawHeightCameras = transform.parent.GetComponentsInChildren<DrawHeightCamera>();

        var top = drawHeightCameras[0];
        var bottom = drawHeightCameras[1];
        var back = drawHeightCameras[2];
        var right = drawHeightCameras[3];
        var forward = drawHeightCameras[4];
        var left = drawHeightCameras[5];

        var x = Vector2.right;
        var y = Vector2.up;

        // top
        top.stitchingUp.uDir = x;
        top.stitchingUp.vDir = y;

        top.stitchingDown.uDir = x;
        top.stitchingDown.vDir = y;

        top.stitchingRight.uDir = -y;
        top.stitchingRight.vDir = x;

        top.stitchingLeft.uDir = y;
        top.stitchingLeft.vDir = -x;

        // back
        back.stitchingUp.uDir = x;
        back.stitchingUp.vDir = y;

        back.stitchingDown.uDir = x;
        back.stitchingDown.vDir = y;

        back.stitchingRight.uDir = x;
        back.stitchingRight.vDir = y;

        back.stitchingLeft.uDir = x;
        back.stitchingLeft.vDir = y;

        // right
        right.stitchingUp.uDir = y;
        right.stitchingUp.vDir = -x;

        right.stitchingDown.uDir = -y;
        right.stitchingDown.vDir = x;

        right.stitchingRight.uDir = x;
        right.stitchingRight.vDir = y;

        right.stitchingLeft.uDir = x;
        right.stitchingLeft.vDir = y;

        // left
        left.stitchingUp.uDir = -y;
        left.stitchingUp.vDir = x;

        left.stitchingDown.uDir = y;
        left.stitchingDown.vDir = -x;

        left.stitchingRight.uDir = x;
        left.stitchingRight.vDir = y;

        left.stitchingLeft.uDir = x;
        left.stitchingLeft.vDir = y;

        // bottom
        bottom.stitchingUp.uDir = x;
        bottom.stitchingUp.vDir = y;

        bottom.stitchingDown.uDir = x;
        bottom.stitchingDown.vDir = y;

        bottom.stitchingRight.uDir = y;
        bottom.stitchingRight.vDir = -x;

        bottom.stitchingLeft.uDir = -y;
        bottom.stitchingLeft.vDir = x;

        // forward
        forward.stitchingUp.uDir = x;
        forward.stitchingUp.vDir = y;

        forward.stitchingDown.uDir = x;
        forward.stitchingDown.vDir = y;

        forward.stitchingRight.uDir = -x;
        forward.stitchingRight.vDir = -y;

        forward.stitchingLeft.uDir = -x;
        forward.stitchingLeft.vDir = -y;

        // 上4
        // StitchingCase.Top_Forward:
        top.stitchingUp.setHeightTexture(top.keepTexture, forward.keepTexture);
        forward.stitchingDown.setHeightTexture(forward.keepTexture, top.keepTexture);

        // case StitchingCase.Top_Right:
        top.stitchingRight.setHeightTexture(top.keepTexture, right.keepTexture);
        right.stitchingUp.setHeightTexture(right.keepTexture, top.keepTexture);

        // case StitchingCase.Top_Back:
        top.stitchingDown.setHeightTexture(top.keepTexture, back.keepTexture);
        back.stitchingUp.setHeightTexture(back.keepTexture, top.keepTexture);

        // case StitchingCase.Top_Left:
        top.stitchingLeft.setHeightTexture(top.keepTexture, left.keepTexture);
        left.stitchingUp.setHeightTexture(left.keepTexture, top.keepTexture);


        // 中4
        // case StitchingCase.Back_Right:
        back.stitchingRight.setHeightTexture(back.keepTexture, right.keepTexture);
        right.stitchingLeft.setHeightTexture(right.keepTexture, back.keepTexture);

        // case StitchingCase.Right_Forward:
        right.stitchingRight.setHeightTexture(right.keepTexture, forward.keepTexture);
        forward.stitchingLeft.setHeightTexture(forward.keepTexture, right.keepTexture);

        // case StitchingCase.Forward_Left:
        forward.stitchingRight.setHeightTexture(forward.keepTexture, left.keepTexture);
        left.stitchingLeft.setHeightTexture(left.keepTexture, forward.keepTexture);

        // case StitchingCase.Left_Back:
        left.stitchingRight.setHeightTexture(left.keepTexture, back.keepTexture);
        back.stitchingLeft.setHeightTexture(back.keepTexture, left.keepTexture);


        //下4
        // case StitchingCase.Bottom_Forward:
        bottom.stitchingDown.setHeightTexture(bottom.keepTexture, forward.keepTexture);
        forward.stitchingUp.setHeightTexture(forward.keepTexture, bottom.keepTexture);

        // case StitchingCase.Bottom_Right:
        bottom.stitchingRight.setHeightTexture(bottom.keepTexture, right.keepTexture);
        right.stitchingDown.setHeightTexture(right.keepTexture, bottom.keepTexture);

        // case StitchingCase.Bottom_Back:
        bottom.stitchingUp.setHeightTexture(back.keepTexture, bottom.keepTexture);
        back.stitchingDown.setHeightTexture(bottom.keepTexture, back.keepTexture);

        // case StitchingCase.Bottom_Left:
        bottom.stitchingLeft.setHeightTexture(bottom.keepTexture, left.keepTexture);
        left.stitchingDown.setHeightTexture(left.keepTexture, bottom.keepTexture);
    }

    public SphereTerrain[] sphereTerrains;

    public void useBrush(bool value)
    {
        foreach (var sTerrain in sphereTerrains)
            sTerrain.useBrush(value);
    }

    public Vector3 getSphereWorldCenter()
    {
        return transform.position;
    }

    public void activeStitching(StitchingCase s, bool value)
    {
        var top = drawHeightCameras[0];
        var bottom = drawHeightCameras[1];
        var back = drawHeightCameras[2];
        var right = drawHeightCameras[3];
        var forward = drawHeightCameras[4];
        var left = drawHeightCameras[5];

        switch (s)
        {
            // 上4
            case StitchingCase.Top_Forward:
                top.stitchingUp.gameObject.SetActive(value);
                forward.stitchingDown.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Right:
                top.stitchingRight.gameObject.SetActive(value);
                right.stitchingUp.gameObject.SetActive(value);
                break;
            case StitchingCase.Top_Back:
                top.stitchingDown.gameObject.SetActive(value);
                back.stitchingUp.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Left:
                top.stitchingLeft.gameObject.SetActive(value);
                left.stitchingUp.gameObject.SetActive(value);
                break;

            // 中4
            case StitchingCase.Back_Right:
                back.stitchingRight.gameObject.SetActive(value);
                right.stitchingLeft.gameObject.SetActive(value);
                break;

            case StitchingCase.Right_Forward:
                right.stitchingRight.gameObject.SetActive(value);
                forward.stitchingLeft.gameObject.SetActive(value);
                break;
            case StitchingCase.Forward_Left:
                forward.stitchingRight.gameObject.SetActive(value);
                left.stitchingLeft.gameObject.SetActive(value);
                break;
            case StitchingCase.Left_Back:
                left.stitchingRight.gameObject.SetActive(value);
                back.stitchingLeft.gameObject.SetActive(value);
                break;

            //下4
            case StitchingCase.Bottom_Forward:
                bottom.stitchingDown.gameObject.SetActive(value);
                forward.stitchingUp.gameObject.SetActive(value);
                break;
            case StitchingCase.Bottom_Right:
                bottom.stitchingRight.gameObject.SetActive(value);
                right.stitchingDown.gameObject.SetActive(value);
                break;
            case StitchingCase.Bottom_Back:
                bottom.stitchingUp.gameObject.SetActive(value);
                back.stitchingDown.gameObject.SetActive(value);
                break;
            case StitchingCase.Bottom_Left:
                bottom.stitchingLeft.gameObject.SetActive(value);
                left.stitchingDown.gameObject.SetActive(value);
                break;
        }
    }
}
