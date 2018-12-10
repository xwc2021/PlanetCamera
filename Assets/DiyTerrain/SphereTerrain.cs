using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTerrain : MonoBehaviour
{
    public Transform pieceOwner;
    public GameObject p129;
    public GameObject p128;
    public GameObject p_128x_129y;
    public GameObject p_129x_128y;
    public Material material;
    public Brush fillBrush;
    public Brush dotBrush;

    public void updateBrushStrength(float strength)
    {
        dotBrush.height = strength;
    }

    public void updateBrushSzie(float size)
    {
        dotBrush.transform.localScale = new Vector3(size, 1.0f, size);
    }

    public void clearHeight(bool clear)
    {
        fillBrush.gameObject.SetActive(clear);
    }

    public Vector3 getPlaneNormal()
    {
        return transform.up;
    }

    public Vector3 getPlanePoint()
    {
        var point = new Vector3(0.0f, 510.0f, 0.0f);
        return transform.TransformPoint(point);
    }

    public void useBrush(bool value)
    {
        dotBrush.gameObject.SetActive(value);
    }

    public void setBrushLocalPos(Vector3 hitPointWorld)
    {
        dotBrush.transform.localPosition = transform.InverseTransformPoint(hitPointWorld);
    }

    public void updateLocalPos()
    {
        foreach (var piece in sphereTerrainPieces)
        {
            piece.updateLocalPos();
        }
    }

    public void updateHeightTexture(Texture heightTexture)
    {
        foreach (var piece in sphereTerrainPieces)
        {
            piece.updateHeightTexture(heightTexture);
        }
    }

    public void create64Piece()
    {
        var offset = new Vector3(-448.0f, 511.5f, -448.0f) + new Vector3(0.5f, 0.0f, 0.5f);
        var xStep = new Vector3(128.0f, 0.0f, 0.0f);
        var zStep = new Vector3(0.0f, 0.0f, 128.0f);
        for (var z = 0; z < 8; ++z)
        {
            for (var x = 0; x < 8; ++x)
            {
                GameObject prefab = null;
                if (x == 7 && z == 7)
                    prefab = p128;
                else if (x == 7)
                    prefab = p_128x_129y;
                else if (z == 7)
                    prefab = p_129x_128y;
                else
                    prefab = p129;

                var pos = offset + xStep * x + zStep * z;
                var obj = Instantiate<GameObject>(prefab, pos, Quaternion.identity);
                obj.transform.parent = pieceOwner;

                obj.transform.name = string.Format("piece({0},{1})", x, z);
            }
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    SphereTerrainPiece[] sphereTerrainPieces;
    void Start()
    {
        sphereTerrainPieces = this.GetComponentsInChildren<SphereTerrainPiece>();
        foreach (var piece in sphereTerrainPieces)
        {
            var mRender = piece.GetComponent<MeshRenderer>();
            mRender.material = material;
        }
    }
}
