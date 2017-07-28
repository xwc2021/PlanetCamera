using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagDefined : MonoBehaviour {

    public static readonly string player = "player";
    public static readonly string npc = "npc";
    public static readonly string monster = "monster";

    public static bool canOnMovableSet(string tag)
    {
        if (tag == TagDefined.player || tag == TagDefined.npc || tag == TagDefined.monster)
            return true;
        else
            return false;
    }
}
