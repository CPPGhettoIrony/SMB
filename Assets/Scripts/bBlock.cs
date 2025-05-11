using System.Collections;
using UnityEngine;

/*
    El siguiente código corresponde a los bloques de ladrillos que pueden golpearse desde abajo.
*/

public class bBlock : MonoBehaviour, Block
{

    /*
        En los juegos de plataformas de Mario, al golpear un bloque por debajo, este responde moviéndose hacia arriba,
        Este vector almacena la posición inicial en la que se encuentra.
        A la posición del bloque en cada fotograma se le aplica la función Lerp para moverse hacia dicha posición inicial, de esta manera, al golpearlo, con sólamente
        mover el bloque hacia arriba, este volverá a la posición anterior, haciendo dicha animación.
    */
    Vector3 currentPos;
    
    // Esta es la posición que toma el bloque tras ser golpeado, (un poco más arriba).
    Vector3 hitPos;

    // En este caso, los bloques de ladrillos, al romperse, expulsan partículas para dar la ilusión de ser rotos.
    public ParticleSystem particles;
    

    // Cuando el bloque es golpeado, antes de ser destruido debe activar el sistema de partículas anterior.
    // Mientras este esté activado, el bloque debe "desaparecer", desactivando el SpriteRenderer y el BoxCollider2D
    SpriteRenderer sprite;
    BoxCollider2D  box;

    public void Start() {

        // Se inicializan las variables anteriores

        currentPos  = transform.position; 
        hitPos      = transform.position + Vector3.up * 0.25f;

        sprite      = GetComponent<SpriteRenderer>();
        box         = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        // Se hace lerp con la posición actual hacia la posición inicial
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 10);
    }

    public void hit() {
        // Cuando el bloque es golpeado desde abajo, se mueve para que el "Lerp" aplique la animación descrita anteriormente.
        transform.position = hitPos;
        StartCoroutine(getHit());
    }

    IEnumerator getHit() {

        // Esperar un tiempo pequeño para que la animación de golpe (con Lerp), pueda apreciarse.
        yield return new WaitForSeconds(0.06f);

        // Hacer el bloque invisible, desactivar colisiones
        sprite.enabled = false;
        box.enabled    = false;

        // Activar el sistema de partículas
        particles.Play();

        // Esperar a que acabe
        yield return new WaitForSeconds(1);

        // Destruir el bloque
        Destroy(gameObject);
    }
}
