using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour {

    void OnTriggerStay2D(Collider2D other) {
        if (other.GetComponent<PlayerController>() == null) {
            return;
        }
        if (Input.GetKeyDown("return")) {
            GameObject.Find("LevelGenerator").GetComponent<LevelGeneration>().generateFlag = true;
        }
    }

}
