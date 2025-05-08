using UnityEngine;

public class Boo : MonoBehaviour
{
    
    public GameObject player;
    Player playerComponent;
    float speed = 0, maxSpeed = 3;

    SpriteRenderer  spriteRenderer;
    Animator        animator;

    int face = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerComponent = player.GetComponent<Player>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        animator        = GetComponent<Animator>();
    }

    void Update()
    {
        face = (player.transform.position.x > transform.position.x)? 1 : -1;
        spriteRenderer.flipX = (face > 0);
        animator.SetBool("boo_shy", face == -playerComponent.face);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(face == -playerComponent.face) {
            speed = 0;
            return;
        }

        transform.position += Vector3.Normalize(player.transform.position - transform.position) * speed * Time.deltaTime;
        speed = (speed < maxSpeed)? speed + 0.2f : speed;
    }
}
