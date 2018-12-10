using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTerrainPiece : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public void updateHeightTexture(Texture heightTexture)
    {
        var m = meshRenderer.material;
        m.SetTexture("_HeightTex", heightTexture);
    }

    public void updateLocalPos()
    {
        var m = meshRenderer.material;
        m.SetVector("_local_pos", transform.localPosition);
    }

    void updateMeshAABB()
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        var bounds = meshFilter.sharedMesh.bounds;
        var nMin = toSphere(bounds.min);
        var nMax = toSphere(bounds.max);

        var newCenter = (nMax + nMin) / 2;
        var size = nMax - nMin;
        // print(size);
        meshFilter.mesh.bounds = new Bounds(newCenter, 2.0f * size);
        // print("modify");
    }

    void Start()
    {
        this.updateMeshAABB();
    }

    // Update is called once per frame
    void Update()
    {
        this.updateLocalPos();
    }

    Vector3 toSphere(Vector3 v)
    {
        var localV = v + transform.localPosition;
        var nV = localV.normalized;
        var R = SphereTerrain.HalfBoxWidth;
        return nV * R - transform.localPosition;
    }
}
