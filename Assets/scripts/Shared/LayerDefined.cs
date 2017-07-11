using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerDefined  {

    public static int Simulate = LayerMask.NameToLayer("Simulate");
    public static int GravitySensor = LayerMask.NameToLayer("GravitySensor");
    public static int ground = LayerMask.NameToLayer("ground");
    public static int Block = LayerMask.NameToLayer("Block");
    public static int canJump = LayerMask.NameToLayer("canJump");
}
