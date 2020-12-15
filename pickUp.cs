using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    public bool colliding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Glove") == false)
        {
            colliding = true;
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

    }

    void OnTriggerExit(Collider c)
    {
        colliding = false;
        //var restoreVelocity = gameObject.GetComponent<Rigidbody>().velocity;
        //restoreVelocity.y = 0;
        //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //gameObject.GetComponent<Rigidbody>().AddForce(restoreVelocity);
    }
}
