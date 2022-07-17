using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnabler : MonoBehaviour
{
    [SerializeField] GameObject uiElement;
    
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name != "Player(Clone)"){
            return;
        }
        uiElement.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.name != "Player(Clone)"){
            return;
        }
        uiElement.SetActive(false);
    }
}
