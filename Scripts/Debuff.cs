using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff
{
    public float intensity;
    public float healthLoss;
    public float speedLoss;
    public float gravLoss;
    public float duration;

    public Debuff(float h, float ss, float gs, float d){
        healthLoss = h;
        speedLoss = ss;
        gravLoss = gs;
        duration = d;
    }
}
