using UnityEngine;

/*
    Uno de los enemigos más notorios de la franquicia de Super Mario son los fantasmas que te acechan cuando no los miras,
    estos se llaman "Boos".
*/

public class Boo : MonoBehaviour
{
    
    // Referencia al jugador que deben seguir
    public GameObject player;

    // Script del jugador, este contiene una variable pública que indica a qué dirección está mirando
    Player playerComponent;

    // Velocidad en el instante, y la máxima velocidad permitida.
    float speed = 0, maxSpeed = 3;

    // Renderer del Sprite (para girarlo dependiendo de a qué dirección se mueva)
    SpriteRenderer  spriteRenderer;

    // Animator, necesario para cambiar el sprite cuando Mario esté mirando al Boo directamente
    Animator        animator;

    // Dirección a la que mira
    int face = -1;


    void Start()
    {
        // Se inicializan las variables
        playerComponent = player.GetComponent<Player>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        animator        = GetComponent<Animator>();
    }

    void Update()
    {
        // El boo siempre debe estar mirando al jugador, dependiendo de si este está más hacia la derecha que el jugador.
        face = (player.transform.position.x > transform.position.x)? 1 : -1;

        // Si mira hacia la derecha, se gira el sprite
        spriteRenderer.flipX = (face > 0);

        // El boo estará detenido si mario lo está mirando (sus direcciones son opuestas)
        animator.SetBool("boo_shy", face == -playerComponent.face);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // El boo se detiene si está siendo mirado
        if(face == -playerComponent.face) {
            speed = 0;
            return;
        }

        // Si no, este aumentará su velocidad hasta alcanzar la máxima mientras se acerca al jugador
        transform.position += Vector3.Normalize(player.transform.position - transform.position) * speed * Time.deltaTime;
        speed = (speed < maxSpeed)? speed + 0.2f : speed;
    }
}
