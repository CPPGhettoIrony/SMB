using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Take()
    {
        Destroy(gameObject);
    }
    
}
