using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//類球形星球專用
public class PlanetGravityGenerator : MonoBehaviour, GroundGravityGenerator
{
    public Transform laddingPlanet;

    public void Awake()
    {
        if (laddingPlanet == null)
        {
            //因為還不知道multiplaer模式要怎麼設定laddingPlanet
            GameObject gameObject = GameObject.FindGameObjectWithTag("nowPlanet");

            if(gameObject!=null)
                laddingPlanet = gameObject.transform;
        }
            
    }

    public Vector3 findGroundUp()
    {
        return (transform.position - laddingPlanet.position).normalized;
    }
}
