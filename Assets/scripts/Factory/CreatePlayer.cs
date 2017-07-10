using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FactoryPlugin
{
    void doIt(GameObject gameObject);
}

public class CreatePlayer : MonoBehaviour {
    public MonoBehaviour[] factoryPloginSocket;
    public PlanetMovable source;
    public GravityGeneratorEnum gge = GravityGeneratorEnum.plane;
	// Use this for initialization
	void Awake () {
        PlanetMovable pm=GameObject.Instantiate<PlanetMovable>(source,transform.position,transform.rotation);
        pm.ResetGravityGenetrator(gge);

        int pluginSide = factoryPloginSocket.Length;
        for (int i = 0; i < pluginSide; i++)
        {
            FactoryPlugin fg = factoryPloginSocket[i] as FactoryPlugin;
            if (fg != null)
                fg.doIt(pm.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
