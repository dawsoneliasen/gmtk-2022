using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] GameObject die;
    List<Spell> availableDice;

    public bool onRope = false;

    void Start(){
        InitDiceList();
    }

    // Update is called once per frame
    void Update(){
        Move();
        Jump();
        ClimbRope();
        ThrowDice();
    }

    void Move(){
        float xDir = 0;
        if(Input.GetKey("a")){
            xDir -= speed * Time.deltaTime;
        }
        if(Input.GetKey("d")){
            xDir += speed * Time.deltaTime;
        }
        if(xDir != 0){
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(5 * Mathf.Sign(xDir), transform.GetComponent<Rigidbody2D>().velocity.y);
        } else{
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x/1.25f, transform.GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    void Jump(){
        if(Input.GetKeyDown("space") && !onRope){
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, -1.5f, 0), new Color(1, 0, 0, 1), 1f);
            if (Physics2D.OverlapCircleAll(transform.position + new Vector3(0, -2.0f, 0), 2.0f).Length > 1) {
            // if (Mathf.Abs(transform.GetComponent<Rigidbody2D>().velocity.y) < 0.01f) {
                transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, 20), ForceMode2D.Impulse);
            }
        }
    }

    void ClimbRope(){
        if(onRope){
            transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.GetComponent<Lifeforce>().defaultGrav = 0;
        }
        
        if(Input.GetKey("w") && onRope){
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 3);
            transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.GetComponent<Lifeforce>().defaultGrav = 0;
        }
        if(Input.GetKey("s") && onRope){
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -3);
            transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.GetComponent<Lifeforce>().defaultGrav = 0;
        }
        if(Input.GetKeyUp("w") && onRope){
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.GetComponent<Lifeforce>().defaultGrav = 0;
        }
        if(Input.GetKeyUp("s") && onRope){
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.GetComponent<Lifeforce>().defaultGrav = 0;
        }
    }

    void ThrowDice(){
        if(Input.GetMouseButtonDown(0) && availableDice.Count > 0){
            int index = Random.Range(0, availableDice.Count);
            GameObject d = GameObject.Instantiate(die);
            d.GetComponent<Die>().spell = availableDice[index];
            //d.GetComponent<Die>().index = availableDice[index];
            d.GetComponent<Die>().castable = true;
            availableDice.RemoveAt(index);
            Vector3 mousePos = Input.mousePosition - new Vector3(Screen.width/2, Screen.height/2, 0);
            d.transform.position = transform.position + new Vector3(1.5f, 0, 0) * Mathf.Sign(mousePos.x);
            float forceX = Random.Range(40, 50) * mousePos.x/Screen.width * 3;
            float forceY = Random.Range(40, 50) * mousePos.y/Screen.height * 3;
            d.GetComponent<Rigidbody2D>().AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            d.GetComponent<Rigidbody2D>().AddTorque(-20 * Mathf.Sign(mousePos.x));
            d.transform.localScale = new Vector3(d.transform.localScale.x * Mathf.Sign(mousePos.x), d.transform.localScale.y, d.transform.localScale.z);
        }
    }

    void CollectDice(GameObject d){
        if(Physics2D.IsTouching(transform.GetComponent<Collider2D>(), d.GetComponent<Collider2D>())){
            if(availableDice.Count < 5){
                availableDice.Add(d.GetComponent<Die>().spell);
                GameObject.Destroy(d);
            }
        }
    }

    void InitDiceList(){
        availableDice = new List<Spell>();
        //for(int i = 0; i < dice.Length; i++){
            //availableDice.Add(i);
        //}
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.collider.GetComponent<Die>() != null){
            CollectDice(col.collider.gameObject);
        }
    }

    void OnDisable(){
        print("Game Over");
    }
}
