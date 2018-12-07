using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTerrain : MonoBehaviour
{
    public Transform from;
    public Transform to;
    public Transform pieceOwner;
    public GameObject piecePrefab;
    public Material material;
    public void updateLocalPos()
    {
        var sphereTerrainPieces = this.GetComponentsInChildren<SphereTerrainPiece>();
        foreach (var piece in sphereTerrainPieces)
        {
            piece.updateLocalPos();
        }
    }

    public void create64Piece()
    {
        var offset = new Vector3(-448.0f, 510.0f, -448.0f);
        var xStep = new Vector3(128.0f, 0.0f, 0.0f);
        var zStep = new Vector3(0.0f, 0.0f, 128.0f);
        for (var z = 0; z < 8; ++z)
        {
            for (var x = 0; x < 8; ++x)
            {
                var pos = offset + xStep * x + zStep * z;
                var obj = Instantiate<GameObject>(this.piecePrefab, pos, Quaternion.identity);
                obj.transform.parent = pieceOwner;

                obj.transform.name = string.Format("piece({0},{1})", x, z);
            }
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        var sphereTerrainPieces = this.GetComponentsInChildren<SphereTerrainPiece>();
        foreach (var piece in sphereTerrainPieces)
        {
            var mRender = piece.GetComponent<MeshRenderer>();
            mRender.material = material;
        }
    }
}
