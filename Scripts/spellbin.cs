using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spellbin
{
    public enum SpellShape{
        Sphere,
        Line,
        Cross,
        Pillar
    }

    public enum SpellElement{
        Order,
        Void,
        Chromatic,
        Gravity,
        Chaos
    }

    public enum BasicDebuffs{
        DoT,
        Slow,
        Poison, //Slow + DoT
        Fire, //Speed + DoT
        Floaty, //Low Gravity
        AntiGrav, //InverseGravity
    }
}
