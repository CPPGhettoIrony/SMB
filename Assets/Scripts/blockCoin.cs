using System.Collections;
using UnityEngine;

// Este código corresponde a la moneda expulsada por un bloque de interrogación al ser golpeado.

/*  En los juegos de Mario, los bloques que contengan monedas no "expulsan" una moneda normal que debería ser recogida con posterioridad,
    en ved de eso, expulsan una moneda que es recogida automáticamente tras "saltar" del bloque.
*/

public class blockCoin : MonoBehaviour
{

    // Velocidad inicial del efecto de "salto" tras ser expulsada del bloque.
    float jump = 0.1f;

    void Start()
    {
        // Esta moneda es recogida al ser expulsada, se añade uno a la cantidad de monedas del jugador
        ++Player.Coins;
        StartCoroutine(routine());
    }

    void Update()
    {
        // Efectuar la animación de "salto"
        transform.Translate(Vector3.up * jump);
        // Para dar la ilusión de gravedad, cada vez se mueve menos hacia arriba
        jump -= 0.005f;
    }

    IEnumerator routine() {
        // La moneda no estará cayendo para siempre, tras un tiempo debe desaparecer
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);  
    }
}
