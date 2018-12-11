using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTerrain : MonoBehaviour
{
    public static float BoxWidth = 1023.0f;
    public static float HalfBoxWidth = 1023.0f * 0.5f;
    public static float R = HalfBoxWidth;

    public Transform pieceOwner;
    public GameObject p129;
    public GameObject p128;
    public GameObject p_128x_129y;
    public GameObject p_129x_128y;
    public Material material;
    public Brush fillBrush;
    public Brush dotBrush;
    public Vector3 localHitPoint;

    public SphereTerrain upPiece;
    public SphereTerrain downPiece;
    public SphereTerrain leftPiece;
    public SphereTerrain rightPiece;

    //靠在邊界上旋轉
    Vector2 getRotAlongBorderLine(Vector2 a, Vector2 b, Vector2 border)
    {
        a = a - border;
        var rot = new Vector2(a.x * b.x - a.y * b.y, b.x * a.y + a.x * b.y);
        return rot + border;
    }

    public void updateNeighborsBrush(bool usingBrush)
    {
        var yz = new Vector2(localHitPoint.y, localHitPoint.z);
        var xy = new Vector2(localHitPoint.x, localHitPoint.y);
        var i = new Vector2(0.0f, 1.0f);

        //upPiece x軸正轉 
        var newYZ = getRotAlongBorderLine(yz, i, new Vector2(SphereTerrain.HalfBoxWidth, SphereTerrain.HalfBoxWidth));

        upPiece.setBrushLocalPosFrom(transform.TransformPoint(new Vector3(localHitPoint.x, newYZ.x, newYZ.y)));
        upPiece.useBrush(usingBrush);

        //debug position
        SphereTerrainBrushController.instance.rotAlongBorder.position = transform.TransformPoint(new Vector3(localHitPoint.x, newYZ.x, newYZ.y));

        //downPiece x軸逆轉 
        newYZ = getRotAlongBorderLine(yz, -i, new Vector2(SphereTerrain.HalfBoxWidth, -SphereTerrain.HalfBoxWidth));// 乘上-i

        downPiece.setBrushLocalPosFrom(transform.TransformPoint(new Vector3(localHitPoint.x, newYZ.x, newYZ.y)));
        downPiece.useBrush(usingBrush);

        //rightPiece z軸逆轉
        var newXY = getRotAlongBorderLine(xy, -i, new Vector2(SphereTerrain.HalfBoxWidth, SphereTerrain.HalfBoxWidth));

        rightPiece.setBrushLocalPosFrom(transform.TransformPoint(new Vector3(newXY.x, newXY.y, localHitPoint.z)));
        rightPiece.useBrush(usingBrush);

        //leftPiece z軸正轉
        newXY = getRotAlongBorderLine(xy, i, new Vector2(-SphereTerrain.HalfBoxWidth, SphereTerrain.HalfBoxWidth));

        leftPiece.setBrushLocalPosFrom(transform.TransformPoint(new Vector3(newXY.x, newXY.y, localHitPoint.z)));
        leftPiece.useBrush(usingBrush);
    }

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
        var point = new Vector3(0.0f, HalfBoxWidth, 0.0f);
        return transform.TransformPoint(point);
    }

    public void useBrush(bool value)
    {
        dotBrush.gameObject.SetActive(value);
    }

    public void setBrushLocalPosFrom(Vector3 hitPointWorld)
    {
        localHitPoint = transform.InverseTransformPoint(hitPointWorld);
        dotBrush.transform.localPosition = localHitPoint;
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
        //長1023
        // 左右端 -511.5~511.5
        var offset = new Vector3(-447.5f, SphereTerrain.HalfBoxWidth, -447.5f);
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
