using System.Collections;
using UnityEngine;

public class blockCoin : MonoBehaviour
{

    float jump = 0.1f;

    void Start()
    {
        ++Player.Coins;
        StartCoroutine(routine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * jump);
        jump -= 0.005f;
    }

    IEnumerator routine() {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);  
    }
}
