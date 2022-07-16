using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTrigger : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.GetComponent<PlayerController>() != null){
            other.gameObject.GetComponent<PlayerController>().onRope = true;
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            other.gameObject.GetComponent<Lifeforce>().defaultGrav = 0;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.GetComponent<PlayerController>() != null){
            StartCoroutine(DelayFall(other.gameObject));
        }
    }
    
    IEnumerator DelayFall(GameObject p){
        yield return new WaitForSeconds(.05f);
        Collider2D[] cols = Physics2D.OverlapCapsuleAll(p.transform.position, new Vector2(1, 1), CapsuleDirection2D.Vertical, 0, 2);
        bool noChains = true;
        foreach(var c in cols){
            if(c.gameObject.GetComponent<RopeTrigger>() != null){
                noChains = false;
                break;
            }
        }
        if(noChains){
            StopCoroutine(DelayFall(p));
        }
        p.GetComponent<PlayerController>().onRope = false;
        p.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        p.GetComponent<Lifeforce>().defaultGrav = 1.5f;
    }
}
