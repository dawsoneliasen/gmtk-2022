using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] GameObject die;
    List<Spell> availableDice;
    int nextDice;
    Die lastThrown;

    public bool onRope = false;

    void Start(){
        InitDiceList();
        nextDice = Random.Range(0, availableDice.Count);
    }

    // Update is called once per frame
    void Update(){
        Move();
        Jump();
        ClimbRope();
        ThrowDice();
        UpdateUI();
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
            if(Physics2D.RaycastAll(transform.position, new Vector3(0, -1f, 0), 1.5f).Length > 1){
                transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, 12), ForceMode2D.Impulse);
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
            int index = nextDice;
            GameObject d = GameObject.Instantiate(die);
            d.GetComponent<Die>().spell = availableDice[index];
            //d.GetComponent<Die>().index = availableDice[index];
            d.GetComponent<Die>().castable = true;
            d.GetComponent<Die>().owner = gameObject;
            lastThrown = d.GetComponent<Die>();
            availableDice.RemoveAt(index);
            nextDice = Random.Range(0, availableDice.Count);
            Vector3 mousePos = Input.mousePosition - new Vector3(Screen.width/2, Screen.height/2, 0);
            d.transform.position = transform.position + new Vector3(.75f, 0, 0) * Mathf.Sign(mousePos.x);
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
                nextDice = Random.Range(0, availableDice.Count);
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

    void UpdateUI(){
        Transform UI = GameObject.Find("UI").transform;
        if(UI == null){
            return;
        }
        //UI.Find("Healthbar/Bar").GetComponent<Image>().fillAmount = transform.GetComponent<Lifeforce>().health/transform.GetComponent<Lifeforce>().maxHealth;
        UI.Find("Healthbar/Bar").GetComponent<RectTransform>().offsetMax = new Vector2((1-transform.GetComponent<Lifeforce>().health/transform.GetComponent<Lifeforce>().maxHealth) * -150, 10);
        if(availableDice.Count > 0){
            UI.Find("NextDiceInfo/Element").GetComponent<TextMeshProUGUI>().text = "Element: " + availableDice[nextDice].element.ToString();
            UI.Find("NextDiceInfo/Sides").GetComponent<TextMeshProUGUI>().text = "Sides: " + availableDice[nextDice].sides.ToString();
            UI.Find("NextDiceInfo/Shape").GetComponent<TextMeshProUGUI>().text = "Shape: " + availableDice[nextDice].shape.ToString();
        } else{
            UI.Find("NextDiceInfo/Element").GetComponent<TextMeshProUGUI>().text = "Element: X";
            UI.Find("NextDiceInfo/Sides").GetComponent<TextMeshProUGUI>().text = "Sides: X";
            UI.Find("NextDiceInfo/Shape").GetComponent<TextMeshProUGUI>().text = "Shape: X";
        }

        if(lastThrown != null){
            if(lastThrown.spellActive){
                UI.Find("LastRoll/Number").GetComponent<TextMeshProUGUI>().text = lastThrown.spell.currSide.ToString();
            } else{
                UI.Find("LastRoll/Number").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(Mathf.Lerp(1, lastThrown.spell.sides, lastThrown.transform.rotation.eulerAngles.z / 360f)).ToString();
            }
        } else{
            UI.Find("LastRoll/Number").GetComponent<TextMeshProUGUI>().text = "X";
        }
    }

    void OnDisable(){
        print("Game Over");
    }
}
