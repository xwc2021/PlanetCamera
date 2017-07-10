using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FactoryPlugin
{
    void doIt(GameObject gameObject);
}

public class CreatePrefab : MonoBehaviour {
    public MonoBehaviour factoryPloginSocket;
    public PlanetMovable source;
    public GravityGeneratorEnum gge = GravityGeneratorEnum.plane;
	// Use this for initialization
	void Awake () {
        PlanetMovable pm=GameObject.Instantiate<PlanetMovable>(source,transform.position,transform.rotation);
        pm.ResetGravityGenetrator(gge);

        FactoryPlugin fg = factoryPloginSocket as FactoryPlugin;
        if (fg == null)
            return;

        fg.doIt(pm.gameObject);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
