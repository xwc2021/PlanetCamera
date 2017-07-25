using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLinedUp : MonoBehaviour,FactoryPlugin {

    public MonoBehaviour factoryPloginSocket;
    public PlanetMovable source;
    public GravityGeneratorEnum gge = GravityGeneratorEnum.plane;
    public int count = 11;
    public int distance = 2;
    public void doIt(GameObject gameObject)
    {
        Transform target = gameObject.transform.Find("followTarget");
        for (int i = 0; i < count; i++)
        {
            Vector3 newPos = transform.position - distance * (i+1) * transform.forward;
            PlanetMovable pm = GameObject.Instantiate<PlanetMovable>(source, newPos, transform.rotation);
            pm.ResetGravityGenetrator(gge);

            pm.gameObject.name = "movable" + i;

            FactoryPlugin fg = factoryPloginSocket as FactoryPlugin;
            if (fg != null)         
                fg.doIt(pm.gameObject);

            FollowerController fc = pm.gameObject.GetComponent<FollowerController>();

            fc.followTarget = target;

            target = pm.transform.Find("followTarget");
        }

    }
}
