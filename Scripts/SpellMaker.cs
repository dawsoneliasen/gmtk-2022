using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using spellbin;

public static class SpellMaker
{
    public static Spell MakeSpellRandom(){
        var shape = (SpellShape)Random.Range(0, 4);
    
        var element = (SpellElement)Random.Range(0, 5);
    
        var radius = Random.Range(3, 5);
    
        var duration = Random.Range(1, 5);

        int sides = 6;
        switch(Random.Range(0, 6)){
            case 0:
                sides = 4;
                break;
            case 1:
                sides = 6;
                break;
            case 2:
                sides = 8;
                break;
            case 3:
                sides = 10;
                break;
            case 4:
                sides = 12;
                break;
            case 5:
                sides = 20;
                break;
        }

        return new Spell(shape, element, Random.Range(1, 5), radius, duration, sides);
    }

    public static Spell MakeSpellLevel(int lvl, int element = -1){
        //lvl should be on a scale from 1-10;
        Spell s = MakeSpellRandom();
        if(element == -1){
            s.element = (SpellElement)Random.Range(0, 5);
        } else{
            s.element = (SpellElement)element;
        }
        s.radius = lvl * Random.Range(.5f, 1.5f);
        float sidesEstimate = Random.Range(lvl/10f * .75f, lvl/10f * 1.25f) * 6;
        int diceType = (int)Mathf.Min(5, Mathf.RoundToInt(sidesEstimate));
        switch(diceType){
            case 0:
                s.sides = 4;
                break;
            case 1:
                s.sides = 6;
                break;
            case 2:
                s.sides = 8;
                break;
            case 3:
                s.sides = 10;
                break;
            case 4:
                s.sides = 12;
                break;
            case 5:
                s.sides = 20;
                break;
        }
        if(Random.Range(0f, 1f) < (lvl * lvl)/100f){
            s.debuffs.Add(MakeElementDebuff(s.element));
        }
        return s;
    }

    public static Debuff MakeElementDebuff(SpellElement e){
        switch(e){
            case SpellElement.Order:
                return new Debuff(Random.Range(0, .1f), Random.Range(0f, .66f), 0, Random.Range(1f, 4f));
            case SpellElement.Void:
                return new Debuff(Random.Range(.33f, .66f), Random.Range(.1f, .25f), .25f, Random.Range(2f, 5f));
            case SpellElement.Chromatic:
                return new Debuff(Random.Range(.5f, 1f), Random.Range(-.5f, 0f), 0, Random.Range(2f, 5f));
            case SpellElement.Gravity:
                return new Debuff(0, Random.Range(.1f, .25f), Random.Range(1.5f, 3f), Random.Range(1f, 3f));
            case SpellElement.Chaos:
                return GetChaosDebuff();
            default:
                return new Debuff(Random.Range(0, .1f), Random.Range(0f, .66f), 0, Random.Range(1f, 4f));
        }
    }

    public static Debuff GetChaosDebuff(){
        int seed = Random.Range(0, 4);
        switch(seed){
            case 0:
                return new Debuff(Random.Range(0, .2f), Random.Range(0f, 1.2f), 0, Random.Range(1f, 4f));
            case 1:
                return new Debuff(Random.Range(.33f, 1.2f), Random.Range(.1f, .5f), Random.Range(.25f, .33f), Random.Range(2f, 5f));
            case 2:
                return new Debuff(Random.Range(.5f, 2f), Random.Range(-.33f, 0f), 0, Random.Range(2f, 5f));
            case 3:
                return new Debuff(0, Random.Range(.1f, .5f), Random.Range(2f, 4f), Random.Range(1f, 3f));
            default:
                return new Debuff(.5f, .5f, .5f, 3);
        }
    }

    public static Spell MakeSpell(SpellShape shape, SpellElement element, float damage, float radius, float duration, int sides){
        return new Spell(shape, element, damage, radius, duration, sides);
    }

    public static Debuff GetDebuff(BasicDebuffs d){
        switch(d){
            case BasicDebuffs.DoT:
                return new Debuff(2, 0, 0, 4);
            case BasicDebuffs.Slow:
                return new Debuff(0, .5f, 0, 3);
            case BasicDebuffs.Poison:
                return new Debuff(1f, .25f, 0, 5);
            case BasicDebuffs.Fire:
                return new Debuff(2, -.25f, 0, 5);
            case BasicDebuffs.Floaty:
                return new Debuff(0, 0, 1f, 6);
            case BasicDebuffs.AntiGrav:
                return new Debuff(0, 0, 3f, 3);
        }
        return new Debuff(0, .5f, 0, 3);
    }

    public static Color GetDiceColor(SpellElement e){
        Debug.Log(e);
        switch(e){
            case SpellElement.Order:
                return new Color(1,1,1,1);
            case SpellElement.Void:
                return new Color(0,0,0,1);
            case SpellElement.Chromatic:
                return new Color(0, 1, .66f, 1);
            case SpellElement.Gravity:
                return new Color(.66f, 0, .66f, 1);
            case SpellElement.Chaos:
                return new Color(.3f, 0f, 0f, 1);
            default:
                return new Color(1, 0, 0, 1);
        }
    }
}
