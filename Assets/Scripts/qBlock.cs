using UnityEngine;

public class qBlock : MonoBehaviour, Block
{
    Vector3 currentPos, hitPos;
    Animator animator;
    BoxCollider2D boxcollider;
    bool isFull = true;

    public GameObject spawn;
    void Start()
    {
        currentPos  = transform.position; 
        hitPos      = transform.position + Vector3.up * 0.25f;
        boxcollider = GetComponent<BoxCollider2D>();
        animator    = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 10);
    }

    public void hit() {
        if (isFull) {
            isFull = false;
            transform.position = hitPos;
            animator.SetTrigger("hit");
            Vector3 pos = transform.position + new Vector3(boxcollider.bounds.size.x/2, boxcollider.bounds.size.y/2, 0f);
            Instantiate(spawn, pos, Quaternion.identity);
        }
    }
}
