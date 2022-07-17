using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using spellbin;
using UnityEngine.VFX;

public class Die : MonoBehaviour
{
    public int index;

    float timeAlive = 0;

    [SerializeField][Range(1,10)] int level = 1;
    [SerializeField][Range(-1,4)] int iElement = -1;
    [SerializeField][Range(-1,4)] int iShape = -1;

    public bool castable = false;
    public bool spellActive = false;
    public bool grabable = false;

    public Spell spell;
    GameObject area;

    public GameObject owner;

    void Start(){
        //spell = SpellMaker.MakeSpellRandom();
        //spell.debuffs.Add(SpellMaker.GetDebuff((BasicDebuffs)Random.Range(0, 6)));
        if(spell == null){
            spell = SpellMaker.MakeSpellLevel(level, iElement, iShape);
        }
        GetComponent<SpriteRenderer>().material.color = SpellMaker.GetDiceColor(spell.element);
        if(!castable){
            grabable = true;
        }
    }

    void Update(){
        if(castable){
            timeAlive += Time.deltaTime;
            CheckRotation();
            if(spellActive){
                Magnitize();
            }
        }
    }

    void CheckRotation(){
        if(timeAlive > .25f && !spellActive && Mathf.Abs(transform.GetComponent<Rigidbody2D>().angularVelocity) < .01){
            spellActive = true;
            CastSpell();
        }
    }

    void CastSpell(){
        if(spell == null){
            return;
        }
        int roll = Random.Range(1, spell.sides);
        print(roll + " out of " + spell.sides);

        foreach(var d in spell.debuffs){
            d.intensity = roll/20f;
        }

        area = GameObject.Instantiate(spell.GetShape(), transform);
        Color col = SpellMaker.GetDiceColor(spell.element);
        col.a = .2f;
        for(int i = 0; i < area.transform.childCount; i++){
            Transform child = area.transform.GetChild(i);
            if(child.GetComponent<DiceArea>() == null){
                continue;
            }
            child.GetComponent<DiceArea>().spell = spell;
            child.GetComponent<DiceArea>().debuffs = spell.debuffs;
            child.GetComponent<SpriteRenderer>().color = col;
        }
        spell.currSide = roll;
        area.transform.localScale *= spell.radius * Mathf.Lerp(1f, 2f, roll/20f);
        area.GetComponent<VisualEffect>().SetFloat("radius", spell.radius * Mathf.Lerp(1f, 2f, roll/20f));
        area.GetComponent<VisualEffect>().SetVector4("color", SpellMaker.GetDiceColor(spell.element));
        StartCoroutine(WaitDuration());
    }

    public void Magnitize(){
        if(owner == null || grabable == false){
            return;
        }
        float distance = Vector3.Distance(transform.position, owner.transform.position);
        if(distance < 2.5){
            transform.position += (owner.transform.position + new Vector3(0, 3, 0) - transform.position)/50;
        }
    }

    IEnumerator WaitDuration(){
        yield return new WaitForSeconds(spell.GetDuration());
        grabable = true;
        GameObject.Destroy(area);
    }
}
