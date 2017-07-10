using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlanet : MonoBehaviour, FactoryPlugin
{
    public Transform laddingPlanet;
    public void doIt(GameObject gameObject)
    {
        Debug.Assert(laddingPlanet != null);
        PlanetGravityGenerator pgg = gameObject.GetComponent<PlanetGravityGenerator>();
        Debug.Assert(pgg != null);
        pgg.laddingPlanet = laddingPlanet;
    }
}
