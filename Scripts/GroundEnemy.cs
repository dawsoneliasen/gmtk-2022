using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public float visionRad = 20;
    public float speed = 5;
    public float jumpPower = 1;

    public float damage = 1;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
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
        float xDiff = target.transform.position.x - transform.position.x;
        float newXVel = speed * ((xDiff)/25 + Mathf.Sign(xDiff)/2);
        Vector2 vel = transform.GetComponent<Rigidbody2D>().velocity;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(vel.x + (1.5f * newXVel - vel.x)/10, vel.y);
        CheckJump();
    }

    void CheckJump(){
        float yDist = target.transform.position.y - transform.position.y;
        float xDist = Mathf.Abs(target.transform.position.x - transform.position.x);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector3 dir3 = target.transform.position - transform.position;
        Vector2 dir2 = new Vector2(dir3.x, dir3.y);
        if(yDist > 3 || (Physics2D.RaycastAll(pos, dir2, 1).Length > 1 && xDist > 1.5)){
            if(Physics2D.RaycastAll(transform.position, new Vector3(0, -1f, 0), 1.1f).Length > 1 && Random.Range(0f, 1f) < .025f){
                transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 5 * jumpPower), ForceMode2D.Impulse);
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D col){
        if(col.collider.gameObject.name == "Player"){
            col.collider.gameObject.GetComponent<Lifeforce>().Damage(damage);
        }
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
}
