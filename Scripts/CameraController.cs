using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject follow;
    [SerializeField] float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (follow == null) {
            return;
        }
        //transform.position = Vector3.MoveTowards(transform.position, follow.transform.position - new Vector3(0, 0, 30), (Vector3.Distance(transform.position, follow.transform.position)/speed) * Time.deltaTime);
        float x = (follow.transform.position.x - transform.position.x)/speed;
        float y = (follow.transform.position.y - transform.position.y)/speed;
        transform.position = new Vector3(transform.position.x + x, transform.position.y + y, -30);
    }
}
