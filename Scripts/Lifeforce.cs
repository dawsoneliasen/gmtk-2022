using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifeforce : MonoBehaviour
{
    [SerializeField] private float health = 50;
    List<Debuff> debuffs;

    public float iFrames = 0;
    public float defaultSpeed = 5;
    public float currSpeed;
    public float defaultGrav = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        debuffs = new List<Debuff>();
        StartCoroutine(DebuffClock());
    }

    // Update is called once per frame
    void Update()
    {
        if(iFrames > 0){
            iFrames -= Time.deltaTime;
        }
        if(health <= 0){
            GameObject.Destroy(gameObject);
        }
    }

    IEnumerator DebuffClock(){
        TriggerDebuffs();
        yield return new WaitForSeconds(.25f);
        StartCoroutine(DebuffClock());
    }

    private void TriggerDebuffs(){
        transform.GetComponent<Rigidbody2D>().gravityScale = defaultGrav;
        currSpeed = defaultSpeed;

        List<Debuff> toRemove = new List<Debuff>();
        foreach(Debuff d in debuffs){
            d.duration -= .25f;
            Damage(d.healthLoss * d.intensity);
            currSpeed -= d.speedLoss * defaultSpeed;
            transform.GetComponent<Rigidbody2D>().gravityScale -= d.gravLoss;
            
            if(d.duration <= 0){
                toRemove.Add(d);
            }
        }

        foreach(var d in toRemove){
            debuffs.Remove(d);
        }
    }

    public void SetHealth(float h){
        health = h;
    }
    public void Damage(float d){
        if(iFrames <= 0){
            health -= d;
            //transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 2.5f), ForceMode2D.Impulse);
            iFrames = .25f;
        }
    }

    public void AddDebuff(Debuff d){
        debuffs.Add(d);
        StopCoroutine(DebuffClock());
        StartCoroutine(DebuffClock());
    }

    void OnDisable(){
        StopCoroutine(DebuffClock());
    }
}
