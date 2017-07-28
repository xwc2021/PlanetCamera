using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagDefined : MonoBehaviour {

    public static string Player = "Player";
    public static string npc = "npc";
    public static string monster = "monster";

    public static bool canOnMovableSet(string tag)
    {
        if (tag == TagDefined.Player || tag == TagDefined.npc || tag == TagDefined.monster)
            return true;
        else
            return false;
    }
}
