using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    Rigidbody2D rb;
    CapsuleCollider2D capsule;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Light2D spotLight;

    public GameObject zone;

    Vector2 crouchSize, dumpSize, normalSize, crouchOffset, normalOffset;

    bool isGrounded = false, isJumping = false, canWallJump = false, changeDirection = false, isCrouching = false, canGetHurt = true, dead = false;
    public bool levelEnd = false;
    float jumpCounter = 0, movement, proxSpeed, hspeed = 0;
    float speed = 10, jumpMax = 15;
    int face = 1;

    public TextMeshProUGUI coinText;
    public Image healthHUD;
    public Sprite[] healthSpr;

    public static int Coins = 0;
    public static float xPos = 0;
    int HP = 4;

    void Start()
    {
        rb              = GetComponent<Rigidbody2D>();
        capsule         = GetComponent<CapsuleCollider2D>();
        animator        = GetComponent<Animator>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        spotLight       = GetComponent<Light2D>();

        normalOffset    = capsule.offset;
        crouchOffset    = capsule.offset;
        crouchOffset.y  = 0.7f;

        normalSize      = capsule.size;
        crouchSize      = capsule.size;
        crouchSize.y    = capsule.size.y/2;
        dumpSize        = capsule.size;
        dumpSize.y      = capsule.size.y * 0.8f; 

        StartCoroutine(Begin());
    }

    void Update() {

        animator.SetFloat("vspeed", rb.linearVelocityY);
        animator.SetFloat("hspeed", Mathf.Abs(rb.linearVelocityX));
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("jump", isJumping);
        animator.SetBool("chdir", changeDirection);
        animator.SetBool("crouch", isCrouching);

        healthHUD.sprite = healthSpr[HP];

        if (levelEnd) {
            proxSpeed = 0;
            spriteRenderer.flipX = false;
            return;
        }

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

        coinText.text = "x " + Coins;
        
    }

    void FixedUpdate()
    {

        xPos = transform.position.x;

        if(dead) {
            rb.linearVelocityY =  jumpMax-jumpCounter;
            if(jumpCounter<=jumpMax*2) jumpCounter += 0.4f;
            return;
        }

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

        rb.linearVelocityX = hspeed;

        if (rb.transform.position.y < zone.transform.position.y - zone.GetComponent<BoxCollider2D>().bounds.size.y/2 - capsule.bounds.size.y) 
            StartCoroutine(Death());
    }

    void collideWithFloor(Collision2D collision) {

        isGrounded = true; 

        foreach (ContactPoint2D point in collision.contacts) {
            //Debug.Log("" + point.point.y + ", " + rb.transform.position.y);
            if(point.point.y >= rb.transform.position.y + capsule.size.y * 0.8)
                isJumping = false;
            if(point.point.y >= rb.transform.position.y) 
                canWallJump = false;
        }

    } 

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.layer) {

            case 11:

                int type = collision.gameObject.GetComponent<Powerup>().getType();

                switch(type) {
                    case 1:
                        if(HP < 4) ++HP;
                    break;
                    case 2:
                        takeDamage();
                    break;
                }

                Destroy(collision.gameObject);

            break;

            case 10:

                BoxCollider2D bc =  collision.gameObject.GetComponent<BoxCollider2D>();
                float yHitLine =  bc.transform.position.y - bc.bounds.size.y - capsule.size.y + 0.1f;

                if (isJumping && rb.transform.position.y > yHitLine)
                        collision.gameObject.GetComponent<Block>().hit();
                collideWithFloor(collision);

                break;

            case 3:
                collideWithFloor(collision);
                break;

            case 9:

                Rigidbody2D erb = collision.gameObject.GetComponent<Rigidbody2D>();
                BoxCollider2D ebc =  collision.gameObject.GetComponent<BoxCollider2D>();
                Enemy e = collision.gameObject.GetComponent<Enemy>();
                float yDamageLine =  erb.transform.position.y - ebc.bounds.size.y * 0.75f;

                if (!isGrounded && rb.transform.position.y > yDamageLine) {
                    e.kill();
                    isJumping = true;
                    break;
                }

                takeDamage();

            break;
        }
    }

    void takeDamage() {
        if (!canGetHurt) return;
        if(HP <= 1) {
            StartCoroutine(Death());
            return;
        }
        StartCoroutine(Hurt());
    }

    IEnumerator Death() {
        HP = 0;
        jumpCounter = 0;
        animator.SetTrigger("dead");
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        dead = true;
        StartCoroutine(End());
        yield break;
    }

    IEnumerator Hurt() {
        canGetHurt = false;
        --HP;
        for(int i=0; i<10; ++i) {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, (i%2==0)?0.9f:0.5f);
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.color = Color.white;
        canGetHurt = true;
    }

    public void endLevel() {
        StartCoroutine(End());
    }

    IEnumerator End()
    {
        spotLight.enabled = true;

        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;

        // Store initial and target values
        float initialInnerRadius = 40f;
        float targetInnerRadius = 0f;

        float initialOuterRadius = 41f;
        float targetOuterRadius = 0.01f;

        // Smoothly transition over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            spotLight.pointLightInnerRadius = Mathf.Lerp(initialInnerRadius, targetInnerRadius, t);
            spotLight.pointLightOuterRadius = Mathf.Lerp(initialOuterRadius, targetOuterRadius, t);

            yield return null;
        }


        // Wait before loading the scene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("main");
    }

    IEnumerator Begin() {

        spotLight.enabled = true;

        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;

        // Store initial and target values
        float initialInnerRadius = 0f;
        float targetInnerRadius = 40f;

        float initialOuterRadius = 0.01f;
        float targetOuterRadius = 41f;

        // Smoothly transition over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            spotLight.pointLightInnerRadius = Mathf.Lerp(initialInnerRadius, targetInnerRadius, t);
            spotLight.pointLightOuterRadius = Mathf.Lerp(initialOuterRadius, targetOuterRadius, t);

            yield return null;
        }

        spotLight.enabled = false;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.layer){
            case 7:
                collision.gameObject.GetComponent<Coin>().Take();
                ++Coins;
            break;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3 || collision.gameObject.layer == 10) 
            isGrounded = false;    
    }

    public void Move(InputAction.CallbackContext context) {
        if(levelEnd) return;
        movement = context.ReadValue<Vector2>().x;
        if(movement != 0)
            face = (int)(movement/Math.Abs(movement));
    }

    public void Jump(InputAction.CallbackContext context) {
        if(levelEnd) return;
        if(isGrounded && context.performed) {
            isJumping = true;
            if(canWallJump) 
                hspeed = -face * speed;
            jumpCounter = 0;
        } if(context.canceled)
            isJumping = false;
    }

    public void Sprint(InputAction.CallbackContext context) {
        if(levelEnd) return;
        if(context.performed) 
            speed = 20;
        else if(context.canceled)
            speed = 10;
    }

    public void Crouch(InputAction.CallbackContext context) {
        if(levelEnd) return;
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
