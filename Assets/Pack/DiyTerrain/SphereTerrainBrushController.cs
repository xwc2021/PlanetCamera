﻿using System.Collections;
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

    public IEnumerator doClear()
    {
        clear = true;
        yield return new WaitForEndOfFrame();
        clear = false;
    }

    public void doStitch()
    {
        StartCoroutine(stitching());
    }

    IEnumerator refreshStitchTexture()
    {
        // 設定heightmap
        topCamera.writeForStitch = true;
        bottomCamera.writeForStitch = true;
        backCamera.writeForStitch = true;
        rightCamera.writeForStitch = true;
        forwardCamera.writeForStitch = true;
        leftCamera.writeForStitch = true;

        yield return new WaitForEndOfFrame();

        topCamera.writeForStitch = false;
        bottomCamera.writeForStitch = false;
        backCamera.writeForStitch = false;
        rightCamera.writeForStitch = false;
        forwardCamera.writeForStitch = false;
        leftCamera.writeForStitch = false;

        // 上4
        // StitchingCase.Top_Forward:
        topCamera.stitchingUp.setHeightTexture(topCamera.stitchTexture, forwardCamera.stitchTexture);
        forwardCamera.stitchingDown.setHeightTexture(forwardCamera.stitchTexture, topCamera.stitchTexture);

        // case StitchingCase.Top_Right:
        topCamera.stitchingRight.setHeightTexture(topCamera.stitchTexture, rightCamera.stitchTexture);
        rightCamera.stitchingUp.setHeightTexture(rightCamera.stitchTexture, topCamera.stitchTexture);

        // case StitchingCase.Top_Back:
        topCamera.stitchingDown.setHeightTexture(topCamera.stitchTexture, backCamera.stitchTexture);
        backCamera.stitchingUp.setHeightTexture(backCamera.stitchTexture, topCamera.stitchTexture);

        // case StitchingCase.Top_Left:
        topCamera.stitchingLeft.setHeightTexture(topCamera.stitchTexture, leftCamera.stitchTexture);
        leftCamera.stitchingUp.setHeightTexture(leftCamera.stitchTexture, topCamera.stitchTexture);


        // 中4
        // case StitchingCase.Back_Right:
        backCamera.stitchingRight.setHeightTexture(backCamera.stitchTexture, rightCamera.stitchTexture);
        rightCamera.stitchingLeft.setHeightTexture(rightCamera.stitchTexture, backCamera.stitchTexture);

        // case StitchingCase.Right_Forward:
        rightCamera.stitchingRight.setHeightTexture(rightCamera.stitchTexture, forwardCamera.stitchTexture);
        forwardCamera.stitchingRight.setHeightTexture(forwardCamera.stitchTexture, rightCamera.stitchTexture);

        // case StitchingCase.Forward_Left:
        forwardCamera.stitchingLeft.setHeightTexture(forwardCamera.stitchTexture, leftCamera.stitchTexture);
        leftCamera.stitchingLeft.setHeightTexture(leftCamera.stitchTexture, forwardCamera.stitchTexture);

        // case StitchingCase.Left_Back:
        leftCamera.stitchingRight.setHeightTexture(leftCamera.stitchTexture, backCamera.stitchTexture);
        backCamera.stitchingLeft.setHeightTexture(backCamera.stitchTexture, leftCamera.stitchTexture);


        //下4
        // case StitchingCase.Bottom_Forward:
        bottomCamera.stitchingDown.setHeightTexture(bottomCamera.stitchTexture, forwardCamera.stitchTexture);
        forwardCamera.stitchingUp.setHeightTexture(forwardCamera.stitchTexture, bottomCamera.stitchTexture);

        // case StitchingCase.Bottom_Right:
        bottomCamera.stitchingRight.setHeightTexture(bottomCamera.stitchTexture, rightCamera.stitchTexture);
        rightCamera.stitchingDown.setHeightTexture(rightCamera.stitchTexture, bottomCamera.stitchTexture);

        // case StitchingCase.Bottom_Back:
        bottomCamera.stitchingUp.setHeightTexture(bottomCamera.stitchTexture, backCamera.stitchTexture);
        backCamera.stitchingDown.setHeightTexture(backCamera.stitchTexture, bottomCamera.stitchTexture);

        // case StitchingCase.Bottom_Left:
        bottomCamera.stitchingLeft.setHeightTexture(bottomCamera.stitchTexture, leftCamera.stitchTexture);
        leftCamera.stitchingDown.setHeightTexture(leftCamera.stitchTexture, bottomCamera.stitchTexture);

        print("refresh StitchTexture");
    }

    public IEnumerator stitching()
    {
        // 1次 stitch 12個邊
        // yield return refreshStitchTexture();
        // for (var i = 0; i < 12; ++i)
        // {
        //     var s = (StitchingCase)i;
        //     print("stitching => " + s);
        //     stitchOne(s, true);
        // }

        // yield return new WaitForEndOfFrame();

        // for (var i = 0; i < 12; ++i)
        // {
        //     var s = (StitchingCase)i;
        //     stitchOne(s, false);
        // }

        // 1次 stitch 1邊
        for (var i = 0; i < 12; ++i)
        {
            yield return refreshStitchTexture();

            var s = (StitchingCase)i;
            print("stitching => " + s);
            stitchOne(s, true);
            yield return new WaitForEndOfFrame();
            stitchOne(s, false);
        }
    }

    void stitchOne(StitchingCase s, bool value)
    {
        // 設定貼圖

        switch (s)
        {
            // 上4
            case StitchingCase.Top_Forward:
                topTerrain.stitchingUp.gameObject.SetActive(value);
                forwardTerrain.stitchingDown.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Right:
                topTerrain.stitchingRight.gameObject.SetActive(value);
                rightTerrain.stitchingUp.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Back:
                topTerrain.stitchingDown.gameObject.SetActive(value);
                backTerrain.stitchingUp.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Left:
                topTerrain.stitchingLeft.gameObject.SetActive(value);
                leftTerrain.stitchingUp.gameObject.SetActive(value);
                break;

            // 中4
            case StitchingCase.Back_Right:
                backTerrain.stitchingRight.gameObject.SetActive(value);
                rightTerrain.stitchingLeft.gameObject.SetActive(value);
                break;

            case StitchingCase.Right_Forward:
                rightTerrain.stitchingRight.gameObject.SetActive(value);
                forwardTerrain.stitchingRight.gameObject.SetActive(value);
                break;

            case StitchingCase.Forward_Left:
                forwardTerrain.stitchingLeft.gameObject.SetActive(value);
                leftTerrain.stitchingLeft.gameObject.SetActive(value);
                break;

            case StitchingCase.Left_Back:
                leftTerrain.stitchingRight.gameObject.SetActive(value);
                backTerrain.stitchingLeft.gameObject.SetActive(value);
                break;


            //下4
            case StitchingCase.Bottom_Forward:
                bottomTerrain.stitchingDown.gameObject.SetActive(value);
                forwardTerrain.stitchingUp.gameObject.SetActive(value);
                break;

            case StitchingCase.Bottom_Right:
                bottomTerrain.stitchingRight.gameObject.SetActive(value);
                rightTerrain.stitchingDown.gameObject.SetActive(value);
                break;

            case StitchingCase.Bottom_Back:
                bottomTerrain.stitchingUp.gameObject.SetActive(value);
                backTerrain.stitchingDown.gameObject.SetActive(value);
                break;

            case StitchingCase.Bottom_Left:
                bottomTerrain.stitchingLeft.gameObject.SetActive(value);
                leftTerrain.stitchingDown.gameObject.SetActive(value);
                break;
        }

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

    SphereTerrain topTerrain;
    SphereTerrain bottomTerrain;
    SphereTerrain backTerrain;
    SphereTerrain rightTerrain;
    SphereTerrain forwardTerrain;
    SphereTerrain leftTerrain;

    DrawHeightCamera topCamera;
    DrawHeightCamera bottomCamera;
    DrawHeightCamera backCamera;
    DrawHeightCamera rightCamera;
    DrawHeightCamera forwardCamera;
    DrawHeightCamera leftCamera;

    void Start()
    {
        topTerrain = this.sphereTerrains[0];
        bottomTerrain = this.sphereTerrains[1];
        backTerrain = this.sphereTerrains[2];
        rightTerrain = this.sphereTerrains[3];
        forwardTerrain = this.sphereTerrains[4];
        leftTerrain = this.sphereTerrains[5];

        instance = this;
        drawHeightCameras = transform.parent.GetComponentsInChildren<DrawHeightCamera>();

        topCamera = drawHeightCameras[0];
        bottomCamera = drawHeightCameras[1];
        backCamera = drawHeightCameras[2];
        rightCamera = drawHeightCameras[3];
        forwardCamera = drawHeightCameras[4];
        leftCamera = drawHeightCameras[5];

        var U = Vector2.right;
        var V = Vector2.up;

        // top
        topCamera.stitchingUp.neibhborU = U;
        topCamera.stitchingUp.neibhborV = V;
        topCamera.stitchingUp.neibhborOriginal = V;

        topCamera.stitchingDown.neibhborU = U;
        topCamera.stitchingDown.neibhborV = V;
        topCamera.stitchingDown.neibhborOriginal = -V;

        topCamera.stitchingRight.neibhborU = V;
        topCamera.stitchingRight.neibhborV = -U;
        topCamera.stitchingRight.neibhborOriginal = 2 * U;

        topCamera.stitchingLeft.neibhborU = -V;
        topCamera.stitchingLeft.neibhborV = U;
        topCamera.stitchingLeft.neibhborOriginal = -U + V;

        // back
        backCamera.stitchingUp.neibhborU = U;
        backCamera.stitchingUp.neibhborV = V;
        backCamera.stitchingUp.neibhborOriginal = V;

        backCamera.stitchingDown.neibhborU = U;
        backCamera.stitchingDown.neibhborV = V;
        backCamera.stitchingDown.neibhborOriginal = -V;

        backCamera.stitchingRight.neibhborU = U;
        backCamera.stitchingRight.neibhborV = V;
        backCamera.stitchingRight.neibhborOriginal = U;

        backCamera.stitchingLeft.neibhborU = U;
        backCamera.stitchingLeft.neibhborV = V;
        backCamera.stitchingLeft.neibhborOriginal = -U;

        // right
        rightCamera.stitchingUp.neibhborU = -V;
        rightCamera.stitchingUp.neibhborV = U;
        rightCamera.stitchingUp.neibhborOriginal = 2 * V;

        rightCamera.stitchingDown.neibhborU = V;
        rightCamera.stitchingDown.neibhborV = -U;
        rightCamera.stitchingDown.neibhborOriginal = U - V;

        rightCamera.stitchingRight.neibhborU = -U;//修正
        rightCamera.stitchingRight.neibhborV = -V;
        rightCamera.stitchingRight.neibhborOriginal = 2 * U + V;

        rightCamera.stitchingLeft.neibhborU = U;
        rightCamera.stitchingLeft.neibhborV = V;
        rightCamera.stitchingLeft.neibhborOriginal = -U;

        // left
        leftCamera.stitchingUp.neibhborU = V;
        leftCamera.stitchingUp.neibhborV = -U;
        leftCamera.stitchingUp.neibhborOriginal = U + V;

        leftCamera.stitchingDown.neibhborU = -V;
        leftCamera.stitchingDown.neibhborV = U;
        leftCamera.stitchingDown.neibhborOriginal = Vector2.zero;

        leftCamera.stitchingRight.neibhborU = U;
        leftCamera.stitchingRight.neibhborV = V;
        leftCamera.stitchingRight.neibhborOriginal = U;

        leftCamera.stitchingLeft.neibhborU = -U;// 修正
        leftCamera.stitchingLeft.neibhborV = -V;
        leftCamera.stitchingLeft.neibhborOriginal = V;

        // bottom
        bottomCamera.stitchingUp.neibhborU = U;
        bottomCamera.stitchingUp.neibhborV = V;
        bottomCamera.stitchingUp.neibhborOriginal = V;

        bottomCamera.stitchingDown.neibhborU = U;
        bottomCamera.stitchingDown.neibhborV = V;
        bottomCamera.stitchingDown.neibhborOriginal = -V;

        bottomCamera.stitchingRight.neibhborU = -V;
        bottomCamera.stitchingRight.neibhborV = U;
        bottomCamera.stitchingRight.neibhborOriginal = U + V;

        bottomCamera.stitchingLeft.neibhborU = V;
        bottomCamera.stitchingLeft.neibhborV = -U;
        bottomCamera.stitchingLeft.neibhborOriginal = Vector2.zero;

        // forward
        forwardCamera.stitchingUp.neibhborU = U;
        forwardCamera.stitchingUp.neibhborV = V;
        forwardCamera.stitchingUp.neibhborOriginal = V;

        forwardCamera.stitchingDown.neibhborU = U;
        forwardCamera.stitchingDown.neibhborV = V;
        forwardCamera.stitchingDown.neibhborOriginal = -V;

        forwardCamera.stitchingRight.neibhborU = -U;
        forwardCamera.stitchingRight.neibhborV = -V;
        forwardCamera.stitchingRight.neibhborOriginal = 2 * U + V;

        forwardCamera.stitchingLeft.neibhborU = -U;
        forwardCamera.stitchingLeft.neibhborV = -V;
        forwardCamera.stitchingLeft.neibhborOriginal = V;

        StartCoroutine(doClear());
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
        switch (s)
        {
            // 上4
            case StitchingCase.Top_Forward:
                topCamera.stitchingUp.gameObject.SetActive(value);
                forwardCamera.stitchingDown.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Right:
                topCamera.stitchingRight.gameObject.SetActive(value);
                rightCamera.stitchingUp.gameObject.SetActive(value);
                break;
            case StitchingCase.Top_Back:
                topCamera.stitchingDown.gameObject.SetActive(value);
                backCamera.stitchingUp.gameObject.SetActive(value);
                break;

            case StitchingCase.Top_Left:
                topCamera.stitchingLeft.gameObject.SetActive(value);
                leftCamera.stitchingUp.gameObject.SetActive(value);
                break;

            // 中4
            case StitchingCase.Back_Right:
                backCamera.stitchingRight.gameObject.SetActive(value);
                rightCamera.stitchingLeft.gameObject.SetActive(value);
                break;

            case StitchingCase.Right_Forward:
                rightCamera.stitchingRight.gameObject.SetActive(value);
                forwardCamera.stitchingLeft.gameObject.SetActive(value);
                break;
            case StitchingCase.Forward_Left:
                forwardCamera.stitchingRight.gameObject.SetActive(value);
                leftCamera.stitchingLeft.gameObject.SetActive(value);
                break;
            case StitchingCase.Left_Back:
                leftCamera.stitchingRight.gameObject.SetActive(value);
                backCamera.stitchingLeft.gameObject.SetActive(value);
                break;

            //下4
            case StitchingCase.Bottom_Forward:
                bottomCamera.stitchingDown.gameObject.SetActive(value);
                forwardCamera.stitchingUp.gameObject.SetActive(value);
                break;
            case StitchingCase.Bottom_Right:
                bottomCamera.stitchingRight.gameObject.SetActive(value);
                rightCamera.stitchingDown.gameObject.SetActive(value);
                break;
            case StitchingCase.Bottom_Back:
                bottomCamera.stitchingUp.gameObject.SetActive(value);
                backCamera.stitchingDown.gameObject.SetActive(value);
                break;
            case StitchingCase.Bottom_Left:
                bottomCamera.stitchingLeft.gameObject.SetActive(value);
                leftCamera.stitchingDown.gameObject.SetActive(value);
                break;
        }
    }
}
