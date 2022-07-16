using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGrowth : MonoBehaviour
{
    Vector3 targetScale;
    // Start is called before the first frame update
    void Start()
    {
        targetScale = transform.localScale;
        transform.localScale = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.parent.rotation.eulerAngles, Quaternion.identity.eulerAngles);
        if(Mathf.Abs(transform.localScale.x - targetScale.x) > .1f){
            transform.localScale += (targetScale - transform.localScale)/25;
        }
    }
}
