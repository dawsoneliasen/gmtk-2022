using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float visionRad = 20;
    public float speed = 5;
    public float flightChaos = 1;

    public float damage = 1;

    public GameObject target;
    public Vector2 offset;
    public Vector2 velOffset;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RandomizeTargetOffset());
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
        if(target != null){
            ChaseTarget();
        }
    }

    void ChaseTarget(){
        float xDiff = target.transform.position.x - transform.position.x + offset.x;
        float yDiff = target.transform.position.y - transform.position.y + offset.y;
        float newXVel = speed * ((xDiff)/25 + Mathf.Sign(xDiff)/2);
        float newYVel = speed * ((yDiff)/25 + Mathf.Sign(yDiff)/2);
        Vector2 vel = transform.GetComponent<Rigidbody2D>().velocity;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(vel.x + (1.5f * newXVel - vel.x)/10, vel.y + (1.5f * newYVel - vel.y)/10) + velOffset;
    }

    IEnumerator RandomizeTargetOffset(){
        offset = new Vector2(Random.Range(-1f, 1f), Random.Range(0, 2)) * (flightChaos/1.5f);
        velOffset = new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)) * flightChaos/3;
        yield return new WaitForSeconds(Random.Range(5f, 10f) / flightChaos);
        StartCoroutine(RandomizeTargetOffset());
    }

    void FindTarget(){
        target = null;
        GameObject t = GameObject.Find("Player(Clone)");
        
        if(t == null){
            return;
        }

        if(Vector3.Distance(transform.position, t.transform.position) <= visionRad){
            target = t;
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        if(Random.Range(0f, 1f) < .5f){
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-transform.GetComponent<Rigidbody2D>().velocity.x, transform.GetComponent<Rigidbody2D>().velocity.y);
            velOffset.x = -velOffset.x;
        } else{
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, -transform.GetComponent<Rigidbody2D>().velocity.y);
            velOffset.y = -velOffset.y;
        }

        if(col.collider.gameObject.name == "Player(Clone)"){
            col.collider.gameObject.GetComponent<Lifeforce>().Damage(damage);
        }
    }

    void OnDisable(){
        StopCoroutine(RandomizeTargetOffset());
    }
}
