using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
}
