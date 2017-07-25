using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerDefined  {

    public static int Simulate = LayerMask.NameToLayer("Simulate");
    public static int GravitySensor = LayerMask.NameToLayer("GravitySensor");

    //ground和wall會和camera測試碰撞
    public static int ground = LayerMask.NameToLayer("ground");
    public static int wall = LayerMask.NameToLayer("wall");
    public static int groundNotBlockCamera = LayerMask.NameToLayer("groundNotBlockCamera");
    public static int wallNotBlockCamera = LayerMask.NameToLayer("wallNotBlockCamera");
}
