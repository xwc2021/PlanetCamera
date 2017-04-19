using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorTriggerBox : MonoBehaviour {

    public EndlessCorridorManager ecManager;
    void OnTriggerExit(Collider other)
    {
        if (other.name == "player")
        {
            float playerScale = other.transform.localScale.y;
            if (Mathf.Abs(playerScale-2.0f)<float.Epsilon)
            {
                //進行縮小修正
                other.transform.localScale = Vector3.one;
                ecManager.worldReSacle(0.5f);
            }
            else if (Mathf.Abs(playerScale - 0.5f) < float.Epsilon)
            {
                //進行放大修正
                other.transform.localScale = Vector3.one;
                ecManager.worldReSacle(2.0f);
            }
        }

    }
}
