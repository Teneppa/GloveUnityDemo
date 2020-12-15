using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showCollision : MonoBehaviour
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
        if(c.CompareTag("Player") == false)
        {
            colliding = true;
        }
        
    }

    void OnTriggerExit(Collider c)
    {
        colliding = false;
    }
}
