using System.Collections;
using Unity.Mathematics;
using UnityEngine;

/*
    El código que utiliza el enemigo más destacable de la franquicia. Este implementa la interfaz "Enemy", que dispara la función kill
    desde el jugador al ser pisado por este.
*/

public class Goomba : MonoBehaviour, Enemy
{
    
    // Rigidbody y Collider
    Rigidbody2D rb;
    Collider2D cl;

    // Animador para cambiar de sprite/animación
    Animator animator;

    // Los objetos que delimitan hacia dónde puede caminar, al tocarlo, este cambiará de dirección
    // útil para que no se caiga de los precipicios.
    public GameObject pointA, pointB;

    // Referencia al punto al que se dirije actualmente
    GameObject point;

    // Velocidad máxima y actual
    float speed = 4, hspeed = 0;

    // Booleano que indica si el enemigo está muerto
    bool dead = false;

    // Dirección a la que se mueve
    int direction = 1;

    void Start()
    {

        // Inicialización de las variables

        rb          = GetComponent<Rigidbody2D>();
        cl          = GetComponent<Collider2D>();    
        animator    = GetComponent<Animator>();

        // Punto al que se dirigirá al empezar

        point = pointB;
    }

    void FixedUpdate()
    {
        // Si está muerto, no moverse
        if(dead) return;

        // Distancia al punto al que se tiene que mover
        float distance = math.abs(transform.position.x - point.transform.position.x);

        // Al alcanzarlo, este cambiará de dirección y se dirigirá al otro punto
        if(distance < 1) {
            direction = -direction;
            point = (point == pointB)? pointA : pointB;
        }

        // Se setea la velocidad a la que tiene que moverse según la dirección
        hspeed = direction * speed;
        rb.linearVelocityX = hspeed;
    }

    IEnumerator Death() {

        // Se cambia la animación a la de muerte (el enemigo está pisado)
        animator.SetTrigger("stomped");

        // El enemigo pasa a estar muerto
        dead = true;

        // Se detiene todo movimiento
        rb.linearVelocityX = 0;
        rb.linearVelocityY = 0;

        // Se desactiva la gravedad y las colisiones
        cl.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Se mantiene el enemigo pisado e inactivo por un segundo y medio
        yield return new WaitForSeconds(1.5f);

        // Finalmente, se elimina
        Destroy(gameObject);
    }

    // Al pisarse, si no está muerto, se dispara la corrutina de muerte
    public void kill() {
        if(!dead)
            StartCoroutine(Death());
    }
}
