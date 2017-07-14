using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GravityGeneratorEnum { plane, planet, mesh }
public class GravityDirectionMonitor : MonoBehaviour {

    public GravityGeneratorEnum ggEnum;
    GroundGravityGenerator grounGravityGenerator;
    public PlaneGravityGenerator planeGravityGeneratorSocket;
    public PlanetGravityGenerator planetGravityGeneratorSocket;
    public MeshGravityGenerator meshGravityGeneratorSocket;

    // Use this for initialization
    void Awake () {
        ResetGravityGenerator(ggEnum);
    }

    public void ResetGravityGenerator(GravityGeneratorEnum pggEnum)
    {
        ggEnum = pggEnum;
        switch (ggEnum)
        {
            case GravityGeneratorEnum.plane:
                grounGravityGenerator = planeGravityGeneratorSocket as GroundGravityGenerator;
                break;
            case GravityGeneratorEnum.planet:
                grounGravityGenerator = planetGravityGeneratorSocket as GroundGravityGenerator;
                break;
            case GravityGeneratorEnum.mesh:
                grounGravityGenerator = meshGravityGeneratorSocket as GroundGravityGenerator;
                break;
        }
    }

    public void setPlanet(Transform planet)
    {
        planetGravityGeneratorSocket.laddingPlanet = planet;
    }

    public GroundGravityGenerator getGravityGenerator()
    {
        return grounGravityGenerator;
    }
}
