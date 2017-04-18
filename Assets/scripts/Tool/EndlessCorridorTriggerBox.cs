using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorTriggerBox : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        if (other.name == "player")
        {
            float playerScale = other.transform.localScale.y;
            if (playerScale > 1.5)
            {
                //進行縮小修正
                other.transform.localScale = Vector3.one;
                other.gameObject.SendMessage("callWorldReSacle", 0.5);
            }
            else if (playerScale < 0.75)
            {
                //進行放大修正
                other.transform.localScale = Vector3.one;
                other.gameObject.SendMessage("callWorldReSacle", 2.0);
            }
        }

    }
}
