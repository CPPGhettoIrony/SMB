using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    Rigidbody2D rb;
    CapsuleCollider2D capsule;
    Animator animator;

    Vector2 debugCollision = Vector2.zero;

    bool isGrounded = false, isJumping = false, isCrouching = false, isSprinting = false;
    float jumpCounter = 0, movement, proxSpeed, hspeed = 0;
    private float speed = 10, jumpMax = 15;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {

        animator.SetFloat("vspeed", rb.linearVelocityY);
        animator.SetFloat("hspeed", Mathf.Abs(rb.linearVelocityX));
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("jump", isJumping);

        proxSpeed = movement * speed;

        Debug.DrawLine(rb.transform.position, debugCollision, Color.magenta);
    }

    void FixedUpdate()
    {
        if(isJumping) {
            rb.linearVelocityY =  jumpMax-jumpCounter;
            jumpCounter += 0.4f;
            if(jumpCounter>=jumpMax) isJumping = false;
        } 

        if(rb.linearVelocityX != hspeed && rb.linearVelocityX == 0) {
            hspeed = 0;
            Debug.Log("Cant Move");
        }

                if(proxSpeed > rb.linearVelocityX)  hspeed += 0.5f;
        else    if(proxSpeed < rb.linearVelocityX)  hspeed -= 0.5f;

        rb.linearVelocityX = hspeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3) {
            isGrounded = true; 
            foreach (ContactPoint2D point in collision.contacts) {
                Debug.Log("" + point.point.y + ", " + rb.transform.position.y);
                if(point.point.y >= rb.transform.position.y - capsule.size.y)  {
                    debugCollision = point.point;
                } 
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3) 
            isGrounded = false;    
    }


    public void Move(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if(isGrounded && context.performed) {
            isJumping = true;
            jumpCounter = 0;
        } if(context.canceled)
            isJumping = false;
    }
}
