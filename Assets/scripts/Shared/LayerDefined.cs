using UnityEngine;
public class LayerDefined
{
    public static readonly int Simulate = LayerMask.NameToLayer("Simulate");
    public static readonly int GravitySensor = LayerMask.NameToLayer("GravitySensor");

    //ground和wall會和camera測試碰撞
    public static readonly int ground = LayerMask.NameToLayer("ground");
    public static readonly int wall = LayerMask.NameToLayer("wall");
    public static readonly int groundNotBlockCamera = LayerMask.NameToLayer("groundNotBlockCamera");
    public static readonly int wallNotBlockCamera = LayerMask.NameToLayer("wallNotBlockCamera");
}