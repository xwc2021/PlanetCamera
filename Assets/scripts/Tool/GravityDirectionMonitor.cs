using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GravityGeneratorEnum { plane, planet, mesh }
public class GravityDirectionMonitor : MonoBehaviour {

    public GravityGeneratorEnum ggEnum;
    GrounGravityGenerator grounGravityGenerator;
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
                grounGravityGenerator = planeGravityGeneratorSocket as GrounGravityGenerator;
                break;
            case GravityGeneratorEnum.planet:
                grounGravityGenerator = planetGravityGeneratorSocket as GrounGravityGenerator;
                break;
            case GravityGeneratorEnum.mesh:
                grounGravityGenerator = meshGravityGeneratorSocket as GrounGravityGenerator;
                break;
        }
    }

    public void setPlanet(Transform planet)
    {
        planetGravityGeneratorSocket.laddingPlanet = planet;
    }

    public GrounGravityGenerator getGravityGenerator()
    {
        return grounGravityGenerator;
    }
}
