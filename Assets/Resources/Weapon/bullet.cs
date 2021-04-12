using UnityEngine;

public class bullet : MonoBehaviour
{
    //movement
    private Rigidbody rb;
    public float initial;
    public float terminal;
    //acceleration
    public float gravity;
    public float wind;
    //player
    public Transform playerTransform;
    private Transform cam;
    //collisions
    private Collider[] collisions;
    public float hits;
    //destroy
    public float time;

    void Start()
    {
        //player
        cam = playerTransform.GetChild(0);
        //movement
        rb = transform.GetComponent<Rigidbody>();
        rb.velocity = (cam.transform.forward * initial);
    }

    void FixedUpdate()
    {
        Accelerations();
        Collisions();
        Destroy();

    }

    void Accelerations()
    {
        //gravity
        rb.velocity += (cam.transform.up * gravity) * Time.deltaTime;

        if(rb.velocity.x <= -terminal)
        {
            //terminal velocity
            rb.velocity = new Vector3(-terminal, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            //wind
            rb.velocity += (cam.transform.forward * wind) * Time.deltaTime;
        }
    }
    
    void Collisions()
    {
        collisions = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (Collider i in collisions)
        {
            if (i.name == "Target")
            {
                player.multiHits -= 1;
                Destroy(gameObject);
                if (player.multiHits == 0)
                {
                    playerTransform.GetComponent<player>().Win();
                }
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
        if (time >= 1000000)
        {
            Destroy(gameObject);
        }
    }
}
