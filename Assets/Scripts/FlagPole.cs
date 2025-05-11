using System.Collections;
using UnityEngine;

/*
    El final del nivel está marcado por la famosa bandera, Al pasar más alla de esta, baja y se termina el nivel.
*/
public class FlagPole : MonoBehaviour
{

    // Este booleano indica cuándo se dispara la bajada de la bandera, y, por lo tanto, la finalización del nivel.
    bool triggered = false;

    // Mientras este booleano esté a "true", la bandera seguirá cayendo
    bool keepFalling = true;

    // Referencia al jugador, estando pendiente de su posición x
    public GameObject player;

    void Update()
    {
        // Si el jugador alcanza la bandera, se dispara la caída y la finalización del nivel.
        if (player.transform.position.x >= transform.position.x && !triggered)
            StartCoroutine(Fall());
            
        // Mientras esté activado lo anterior y se pueda seguir cayendo, la bandera se moverá hacia abajo
        if (triggered && keepFalling)
            transform.position += Vector3.down * 5 * Time.deltaTime;
    }

    
    IEnumerator Fall() {

        // La bandera, negra y con el logotipo de bowser, cambia a una roja con la M de Mario
        GetComponent<Animator>().SetTrigger("goal");

        // Se dispara la caída de la bandera
        triggered = true;

        // Se pone a true el booleano levelEnd, deteniendo el movimiento de Mario y forzándolo a mirar hacia la izquierda
        player.GetComponent<Player>().levelEnd = true;

        // Se espera dos segundos, para que la bandera pueda caer
        yield return new WaitForSeconds(2);

        // Se termina el nivel y se detiene la caída
        player.GetComponent<Player>().endLevel();
        keepFalling = false;
    }

}
