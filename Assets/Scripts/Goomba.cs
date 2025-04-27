using System.Collections;
using UnityEngine;

public class Goomba : MonoBehaviour, Enemy
{
    
    Rigidbody2D rb;
    Collider2D cl;
    Animator animator;

    float speed = 4, hspeed = 0;
    bool dead = false;
    int direction = 1;

    void Start()
    {
        rb          = GetComponent<Rigidbody2D>();
        cl          = GetComponent<Collider2D>();    
        animator    = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if(dead) return;
        if (rb.linearVelocityX == 0)
            direction = -direction;

        hspeed = direction * speed;
        rb.linearVelocityX = hspeed;
    }

    IEnumerator Death() {
        animator.SetTrigger("stomped");
        dead = true;
        rb.linearVelocityX = 0;
        cl.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public void kill() {
        if(!dead)
            StartCoroutine(Death());
    }
}
