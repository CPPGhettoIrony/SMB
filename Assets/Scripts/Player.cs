using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    Rigidbody2D rb;
    CapsuleCollider2D capsule;
    Animator animator;
    SpriteRenderer spriteRenderer;

    Vector2 debugCollision = Vector2.zero;
    Vector2 crouchSize, dumpSize, normalSize, crouchOffset, dumpOffset, normalOffset;

    //public Camera cam;
    //public bool allowCameraXFollow, allowCameraYFollow;
    //Vector3 cameraPosition;

    bool isGrounded = false, isJumping = false, canWallJump = false, changeDirection = false, isCrouching = false;
    float jumpCounter = 0, movement, proxSpeed, hspeed = 0;
    float speed = 10, jumpMax = 15;
    int face = 1;

    void Start()
    {
        rb              = GetComponent<Rigidbody2D>();
        capsule         = GetComponent<CapsuleCollider2D>();
        animator        = GetComponent<Animator>();
        spriteRenderer  = GetComponent<SpriteRenderer>();

        //cameraPosition  = cam.transform.position;

        normalOffset    = capsule.offset;
        crouchOffset    = capsule.offset;
        crouchOffset.y  = 0.7f;

        normalSize      = capsule.size;
        crouchSize      = capsule.size;
        crouchSize.y    = capsule.size.y/2;
        dumpSize        = capsule.size;
        dumpSize.y      = capsule.size.y * 0.8f; 
    }

    void Update() {

        animator.SetFloat("vspeed", rb.linearVelocityY);
        animator.SetFloat("hspeed", Mathf.Abs(rb.linearVelocityX));
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("jump", isJumping);
        animator.SetBool("chdir", changeDirection);
        animator.SetBool("crouch", isCrouching);

        if(isCrouching && isGrounded) 
            proxSpeed = 0;
        else 
            proxSpeed = movement * speed; 

        //Debug.DrawLine(rb.transform.position, debugCollision, Color.magenta);
        //Debug.DrawLine(rb.transform.position, rb.transform.position + Vector3.left * capsule.size.x/2, Color.red);

        if(Mathf.Abs(hspeed) > 0.1 && !canWallJump)
            spriteRenderer.flipX = changeDirection?(hspeed < 0):(hspeed >= 0);
        else if (canWallJump)
            spriteRenderer.flipX = (face < 0);

        //if(allowCameraXFollow) cameraPosition.x = rb.transform.position.x;
        //if(allowCameraYFollow) cameraPosition.y = rb.transform.position.y;
        //cam.transform.position = cameraPosition;

        changeDirection = proxSpeed * hspeed < 0 && !isCrouching;
        
    }

    void FixedUpdate()
    {
        if(isJumping) {
            rb.linearVelocityY =  jumpMax-jumpCounter;
            jumpCounter += 0.4f;
            if(jumpCounter>=jumpMax) isJumping = false;
        } 

        if(rb.linearVelocityX != hspeed && rb.linearVelocityX == 0 && !canWallJump && !isCrouching) {
            hspeed = 0;
            canWallJump = Mathf.Abs(rb.linearVelocityY) > 0.1;
            if(canWallJump) animator.SetTrigger("walljump");
        }

                if(proxSpeed > rb.linearVelocityX)  hspeed += 0.5f;
        else    if(proxSpeed < rb.linearVelocityX)  hspeed -= 0.5f;

        //Debug.Log(hspeed);

        rb.linearVelocityX = hspeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3) {
            isGrounded = true; 
            foreach (ContactPoint2D point in collision.contacts) {
                //Debug.Log("" + point.point.y + ", " + rb.transform.position.y);
                if(point.point.y >= rb.transform.position.y + capsule.size.y * 0.8)
                    isJumping = false;
                if(point.point.y >= rb.transform.position.y) 
                    canWallJump = false;
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
        if(movement != 0)
            face = (int)(movement/Math.Abs(movement));
    }

    public void Jump(InputAction.CallbackContext context) {
        if(isGrounded && context.performed) {
            isJumping = true;
            if(canWallJump) 
                hspeed = -face * speed;
            jumpCounter = 0;
        } if(context.canceled)
            isJumping = false;
    }

    public void Sprint(InputAction.CallbackContext context) {
        if(context.performed) 
            speed = 20;
        else if(context.canceled)
            speed = 10;
    }

    public void Crouch(InputAction.CallbackContext context) {
        if(context.performed) {
            if(isGrounded) {
                isCrouching = true;
                capsule.size = crouchSize;
                capsule.offset = crouchOffset;
            }
            canWallJump = false;
        }else if(context.canceled) {
            isCrouching = false;
            capsule.size = normalSize;
            capsule.offset = normalOffset;
        }
    }
}
