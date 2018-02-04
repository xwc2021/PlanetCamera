using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SharedTool
{
    public static bool IsGetMainCamera()
    {
        var cam = Camera.current;
        if (!cam)
            return false;

        if (cam != Camera.main)
            return false;

        return true;
    }
}
