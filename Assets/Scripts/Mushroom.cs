using UnityEngine;

// El código siguiente lo comparten todos los champiñones:
//  ·    Al crearse (salir de un bloque tras ser golpeado), dan un pequeño salto hacia arriba y empiezan a moverse hacia la izquierda o derecha
//  ·    Al colisionar con una pared, cambia de dirección

public class Mushroom : MonoBehaviour, Powerup
{
    
    Rigidbody2D rb;

    // El tipo de champiñón:

    //  1 : El champiñón amarillo,  añade 1 a la salud del jugador
    //  2 : El champiñón púrpura,   daña al jugador

    // Para cada champiñón hay un prefab distinto, con sprite distinto y el tipo configurado

    public int type;


    // Velocidad máxima y actual
    float speed = 4, hspeed = 0;

    // Dirección a la que se mueve
    int direction;

    void Start()
    {
        // Inicialización de las variables
        rb          = GetComponent<Rigidbody2D>();
        direction   = Random.Range(0, 1)==1? 1 : -1;

        // Al crearse, salta del bloque
        rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse); 
    }

    void FixedUpdate()
    {
        // Cambiar de dirección si se ha colisionado con el muro (se detiene el movimiento)
        if (rb.linearVelocityX == 0)
            direction = -direction;

        // Setear la velocidad actual dependiendo de la dirección
        hspeed = direction * speed;
        rb.linearVelocityX = hspeed;
    }

    // Retornar el tipo de Powerup
    public int getType() {
        return type;
    }

}
