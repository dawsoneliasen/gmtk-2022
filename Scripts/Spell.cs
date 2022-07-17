using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using spellbin;

public class Spell
{
    public SpellShape shape;
    public SpellElement element;

    public float damage;

    public float radius;
    public float duration;

    public int sides;
    public int currSide = 1;

    public List<Debuff> debuffs;

    public Spell(SpellShape shape, SpellElement element, float damage, float radius, float duration, int sides){
        this.shape = shape;
        this.element = element;
        this.damage = damage;
        this.radius = radius;
        this.duration = duration;
        this.sides = sides;
        debuffs = new List<Debuff>();
    }

    public GameObject GetShape(){
        switch(shape){
            case SpellShape.Sphere:
                return Resources.Load<GameObject>("SpellShapes/Sphere");
            case SpellShape.Line:
                return Resources.Load<GameObject>("SpellShapes/Line");
            case SpellShape.Cross:
                return Resources.Load<GameObject>("SpellShapes/Cross");
            case SpellShape.Pillar:
                return Resources.Load<GameObject>("SpellShapes/Pillar");
            default:
                return Resources.Load<GameObject>("SpellShapes/Sphere");
        }
    }

    public float GetDuration(){
        return duration * currSide/10f;
    }

    public float GetDamage(){
        return damage * currSide/20f;
    }
}
