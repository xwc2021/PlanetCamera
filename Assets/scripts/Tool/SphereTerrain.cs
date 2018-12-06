using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTerrain : MonoBehaviour
{
    public Transform from;
    public Transform to;
    public void updateLocalPos()
    {
        var sphereTerrainPieces = this.GetComponentsInChildren<SphereTerrainPiece>();
        foreach (var piece in sphereTerrainPieces)
        {
            piece.updateLocalPos();
        }
    }
}
