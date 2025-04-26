using System.Collections;
using UnityEngine;

public class bBlock : MonoBehaviour, Block
{

    Vector3 currentPos, hitPos;
    public ParticleSystem particles;
    
    SpriteRenderer sprite;
    BoxCollider2D  box;

    public void Start() {

        currentPos  = transform.position; 
        hitPos      = transform.position + Vector3.up * 0.25f;

        sprite      = GetComponent<SpriteRenderer>();
        box         = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 10);
    }

    public void hit() {
        transform.position = hitPos;
        StartCoroutine(getHit());
    }

    IEnumerator getHit() {
        yield return new WaitForSeconds(0.06f);
        sprite.enabled = false;
        box.enabled    = false;
        particles.Play();
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
