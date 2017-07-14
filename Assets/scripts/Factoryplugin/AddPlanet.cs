using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlanet : MonoBehaviour, FactoryPlugin
{
    public Transform laddingPlanet;
    public void doIt(GameObject gameObject)
    {
        Debug.Assert(laddingPlanet != null);
        GravityDirectionMonitor gdm = gameObject.GetComponentInChildren<GravityDirectionMonitor>();
        Debug.Assert(gdm != null);
        gdm.setPlanet(laddingPlanet);
    }
}
