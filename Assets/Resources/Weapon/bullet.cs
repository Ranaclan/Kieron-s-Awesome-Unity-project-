using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    //values
    public float initial;
    public float gravity;
    //movement
    private Rigidbody rb;
    //collisions
    private Collider[] collisions;
    //destroy
    public float time;
    //player
    public Transform player;
    private Transform cam;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
        //player
        cam = player.GetChild(0);
        //movement
        rb = transform.GetComponent<Rigidbody>();
        rb.velocity = (cam.transform.forward * initial);
    }
        
    void FixedUpdate()
    {
        Gravity();
        Collisions();
        Destroy();

    }

    void Gravity()
    {
        rb.velocity += (transform.up * gravity) * Time.deltaTime;
    }

    void Collisions()
    {
        collisions = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (Collider i in collisions)
        {
            if (i.name == "Target")
            {
                Debug.Log("target");
                Destroy(gameObject);
            }

            if (i.name == "Ground" || transform.position.y <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void Destroy()
    {
        //measures time alive
        time += Time.deltaTime;

        //destroys bullet after time
        if(time >= 1000000)
        {
            //Destroy(gameObject);
        }
    }
}
