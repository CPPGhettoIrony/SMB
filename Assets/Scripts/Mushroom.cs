
using UnityEngine;
public class Mushroom : MonoBehaviour, Powerup
{
    
    Rigidbody2D rb;
    Collider2D cl;

    public int type;

    float speed = 4, hspeed = 0;
    int direction;

    void Start()
    {
        rb          = GetComponent<Rigidbody2D>();
        cl          = GetComponent<Collider2D>();   
        rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse); 

        direction = Random.Range(0, 1)==1? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (rb.linearVelocityX == 0)
            direction = -direction;

        hspeed = direction * speed;
        rb.linearVelocityX = hspeed;
    }

    public int getType() {
        return type;
    }

}
