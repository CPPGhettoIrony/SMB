using UnityEngine;

/*
    El siguiente código corresponde a los bloques de interrogación que contienen monedas o powerups
*/

public class qBlock : MonoBehaviour, Block
{

    /*
        En los juegos de plataformas de Mario, al golpear un bloque por debajo, este responde moviéndose hacia arriba,
        Este vector almacena la posición inicial en la que se encuentra.
        A la posición del bloque en cada fotograma se le aplica la función Lerp para moverse hacia dicha posición inicial, de esta manera, al golpearlo, con sólamente
        mover el bloque hacia arriba, este volverá a la posición anterior, haciendo dicha animación.
    */
    Vector3 currentPos, hitPos;

    // En caso de los bloques de interrogación, estos cambian de animación/sprite al ser golpeados
    Animator animator;

    // El collider se utiliza para obtener la posición en la que  "spawnear" el Powerup o moneda blockCoin
    BoxCollider2D boxcollider;

    // Este booleano indica si no ha sido golpeado antes, oséase, que aún no ha sido "vaciado" de lo que contiene
    bool isFull = true;

    // El prefab a "spawnear" al ser golpeado
    public GameObject spawn;

    void Start()
    {
        // Inicialización de las variables
        currentPos  = transform.position; 
        hitPos      = transform.position + Vector3.up * 0.25f;
        boxcollider = GetComponent<BoxCollider2D>();
        animator    = GetComponent<Animator>();
    }



    void FixedUpdate()
    {
        // Se hace lerp con la posición actual hacia la posición inicial
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 10);
    }

    // Cuando se golpea desde abajo...
    public void hit() {

        // Si aún "contiene" el objeto a spawnear
        if (isFull) {

            // El bloque estará vacío
            isFull = false;

            // Cuando el bloque es golpeado desde abajo, se mueve para que el "Lerp" aplique la animación descrita anteriormente.
            transform.position = hitPos;

            // Cambiar la animación por el sprite de bloque "vacío"
            animator.SetTrigger("hit");

            // Obtener la posición en la que "spawnear" el objeto
            Vector3 pos = transform.position + new Vector3(boxcollider.bounds.size.x/2, boxcollider.bounds.size.y/2, 0f);

            // Insanciar el objeto en la posición obtenida
            Instantiate(spawn, pos, Quaternion.identity);
        }
    }
}
