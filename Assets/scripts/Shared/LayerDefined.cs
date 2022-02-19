using UnityEngine;
public class LayerDefined
{
    public static readonly int Simulate = LayerMask.NameToLayer("Simulate");
    public static readonly int GravitySensor = LayerMask.NameToLayer("GravitySensor");

    public static readonly int Border = LayerMask.NameToLayer("Border");
    public static readonly int BorderBlockCamera = LayerMask.NameToLayer("BorderBlockCamera");
    public static readonly int BorderNoAvoid = LayerMask.NameToLayer("BorderNoAvoid");
    public static readonly int BlockCamera = LayerMask.NameToLayer("BlockCamera");

}