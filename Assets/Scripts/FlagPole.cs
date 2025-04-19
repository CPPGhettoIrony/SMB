using System.Collections;
using UnityEngine;

public class FlagPole : MonoBehaviour
{
    bool triggered = false, keepFalling = true;
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x >= transform.position.x && !triggered)
            StartCoroutine(Fall());
            
        if (triggered && keepFalling)
            transform.position += Vector3.down * 5 * Time.deltaTime;
    }

    
    IEnumerator Fall() {
        GetComponent<Animator>().SetTrigger("goal");
        triggered = true;
        player.GetComponent<Player>().levelEnd = true;
        yield return new WaitForSeconds(2);
        keepFalling = false;
    }

}
