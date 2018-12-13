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
        topTerrain.stitchingDown.gameObject.SetActive(value);
        backTerrain.stitchingUp.gameObject.SetActive(value);

        // case StitchingCase.Top_Left:
        topTerrain.stitchingLeft.gameObject.SetActive(value);
        leftTerrain.stitchingUp.gameObject.SetActive(value);

        // 中4
        // case StitchingCase.Back_Right:
        backTerrain.stitchingRight.gameObject.SetActive(value);
        rightTerrain.stitchingLeft.gameObject.SetActive(value);

        // case StitchingCase.Right_Forward:
        rightTerrain.stitchingRight.gameObject.SetActive(value);
        forwardTerrain.stitchingRight.gameObject.SetActive(value);// forward 的面向不太一樣

        // case StitchingCase.Forward_Left:
        forwardTerrain.stitchingLeft.gameObject.SetActive(value);// forward 的面向不太一樣
        leftTerrain.stitchingLeft.gameObject.SetActive(value);

        // case StitchingCase.Left_Back:
        leftTerrain.stitchingRight.gameObject.SetActive(value);
        backTerrain.stitchingLeft.gameObject.SetActive(value);


        //下4
        // case StitchingCase.Bottom_Forward:
        // bottomTerrain.stitchingDown.gameObject.SetActive(value);
        // forwardTerrain.stitchingUp.gameObject.SetActive(value);

        // case StitchingCase.Bottom_Right:
        // bottomTerrain.stitchingRight.gameObject.SetActive(value);
        // rightTerrain.stitchingDown.gameObject.SetActive(value);

        // case StitchingCase.Bottom_Back:
        // bottomTerrain.stitchingUp.gameObject.SetActive(value);
        // backTerrain.stitchingDown.gameObject.SetActive(value);

        // case StitchingCase.Bottom_Left:
        // bottomTerrain.stitchingLeft.gameObject.SetActive(value);
        // leftTerrain.stitchingDown.gameObject.SetActive(value);
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
        top.stitchingUp.neibhborU = x;
        top.stitchingUp.neibhborV = y;
        top.stitchingUp.neibhborOriginal = y;

        top.stitchingDown.neibhborU = x;
        top.stitchingDown.neibhborV = y;
        top.stitchingDown.neibhborOriginal = -y;

        top.stitchingRight.neibhborU = y;
        top.stitchingRight.neibhborV = -x;
        top.stitchingRight.neibhborOriginal = 2 * x;

        top.stitchingLeft.neibhborU = -y;
        top.stitchingLeft.neibhborV = x;
        top.stitchingLeft.neibhborOriginal = -x + y;

        // back
        back.stitchingUp.neibhborU = x;
        back.stitchingUp.neibhborV = y;
        back.stitchingUp.neibhborOriginal = y;

        back.stitchingDown.neibhborU = x;
        back.stitchingDown.neibhborV = y;
        back.stitchingDown.neibhborOriginal = -y;

        back.stitchingRight.neibhborU = x;
        back.stitchingRight.neibhborV = y;
        back.stitchingRight.neibhborOriginal = x;

        back.stitchingLeft.neibhborU = x;
        back.stitchingLeft.neibhborV = y;
        back.stitchingLeft.neibhborOriginal = -x;

        // right
        right.stitchingUp.neibhborU = -y;
        right.stitchingUp.neibhborV = x;
        right.stitchingUp.neibhborOriginal = 2 * y;

        right.stitchingDown.neibhborU = y;
        right.stitchingDown.neibhborV = -x;
        right.stitchingDown.neibhborOriginal = x - y;

        right.stitchingRight.neibhborU = -x;//修正
        right.stitchingRight.neibhborV = -y;
        right.stitchingRight.neibhborOriginal = 2 * x + y;

        right.stitchingLeft.neibhborU = x;
        right.stitchingLeft.neibhborV = y;
        right.stitchingLeft.neibhborOriginal = -x;

        // left
        left.stitchingUp.neibhborU = y;
        left.stitchingUp.neibhborV = -x;
        left.stitchingUp.neibhborOriginal = x + y;

        left.stitchingDown.neibhborU = -y;
        left.stitchingDown.neibhborV = x;
        left.stitchingDown.neibhborOriginal = Vector2.zero;

        left.stitchingRight.neibhborU = x;
        left.stitchingRight.neibhborV = y;
        left.stitchingRight.neibhborOriginal = x;

        left.stitchingLeft.neibhborU = -x;// 修正
        left.stitchingLeft.neibhborV = -y;
        left.stitchingLeft.neibhborOriginal = y;

        // bottom
        bottom.stitchingUp.neibhborU = x;
        bottom.stitchingUp.neibhborV = y;
        bottom.stitchingUp.neibhborOriginal = y;

        bottom.stitchingDown.neibhborU = x;
        bottom.stitchingDown.neibhborV = y;
        bottom.stitchingDown.neibhborOriginal = -y;

        bottom.stitchingRight.neibhborU = -y;
        bottom.stitchingRight.neibhborV = x;
        bottom.stitchingRight.neibhborOriginal = x + y;

        bottom.stitchingLeft.neibhborU = y;
        bottom.stitchingLeft.neibhborV = -x;
        bottom.stitchingLeft.neibhborOriginal = Vector2.zero;

        // forward
        forward.stitchingUp.neibhborU = x;
        forward.stitchingUp.neibhborV = y;
        forward.stitchingUp.neibhborOriginal = y;

        forward.stitchingDown.neibhborU = x;
        forward.stitchingDown.neibhborV = y;
        forward.stitchingDown.neibhborOriginal = -y;

        forward.stitchingRight.neibhborU = -x;
        forward.stitchingRight.neibhborV = -y;
        forward.stitchingRight.neibhborOriginal = 2 * x + y;

        forward.stitchingLeft.neibhborU = -x;
        forward.stitchingLeft.neibhborV = -y;
        forward.stitchingLeft.neibhborOriginal = y;

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
        forward.stitchingRight.setHeightTexture(forward.keepTexture, right.keepTexture);

        // case StitchingCase.Forward_Left:
        forward.stitchingLeft.setHeightTexture(forward.keepTexture, left.keepTexture);
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
