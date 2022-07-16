using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceArea : MonoBehaviour
{
    public List<Debuff> debuffs;
    public Spell spell;

    void OnTriggerStay2D(Collider2D other){
        Lifeforce l = other.GetComponent<Lifeforce>();
        if(l == null){
            return;
        }
        if(l.iFrames > 0){
            return;
        }
        //print("entering trigger...");
        l.Damage(spell.GetDamage());
        foreach(Debuff d in debuffs){
            l.AddDebuff(d);
        }
    }
}
