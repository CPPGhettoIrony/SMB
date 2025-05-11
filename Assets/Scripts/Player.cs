using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    El código correspondiente al jugador, parte el prefab Mario, que incluye:
        ·   El Jugador
        ·   La cámara y su controlador cinemachine
        ·   La zona que delimita el movimiento de la cámara y el tamaño del nivel
        ·   Un canvas con información sobre los puntos de salud y monedas obtenidas actualmente
*/

public class Player : MonoBehaviour
{

    // El nombre de la escena a la que se transiciona tras terminar un nivel
    public string nextScene;

    // RigidBody y Collider para procesar la física
    Rigidbody2D rb;
    CapsuleCollider2D capsule;

    // Animator y spriteRenderer para cambiar las animaciones del jugador y la dirección a la que mira
    Animator animator;
    SpriteRenderer spriteRenderer;

    // Luz para los efectos de transición tras empezar el nivel, morir o ganar
    Light2D spotLight;

    // La zona especificada anteriormente, al estar por debajo de esta, se asume que Mario ha caído por el precipicio, disparando la corrutina de muerte
    public GameObject zone;

    // Clips de audio para el salto, la obtención de monedas, cuando se recibe daño y la muerte
    // Debido a que el sonido no funciona correctamente en Unity (y en los juegos que se compila con este) desde Manjaro Linux, no he podido implementarlo
    public AudioClip jumpSFX, coinSFX, hurtSFX, deathSFX;

    // Audiosource para la reproducción de sonidos
    AudioSource audioSource;

    // Tamaños del collider al estar (o no) agachado, junto a los offsets a los que se mueve el jugador para evitar errores frutos del cambio de dimensión
    // con respecto al punto pivot del GameObject
    Vector2 crouchSize, normalSize, crouchOffset, normalOffset;

    // Booleano que indica si el jugador está tocando suelo
    bool isGrounded = false;
    
    // Booleano que indica si el jugador está saltando
    bool isJumping = false;
    
    // Booleano que indica si el jugador puede hacer "Walljump"
    bool canWallJump = false;
    
    // Booleano que indica si el jugador está cambiando de dirección mientras corre (para cambiar el sprite al indicado)
    bool changeDirection = false;
    
    // Booleano que indica si el jugador está agachado
    bool isCrouching = false;
    
    // Booleano que indica si el jugador puede recibir daño (para evitar activar la corrutina de daño multiples veces o recibir daño durante fotogramas seguidos)
    bool canGetHurt = true;
    
    // Booleano que indica si el jugador está muerto o no
    bool dead = false;

    // Booleano que indica si se ha terminado el nivel
    public bool levelEnd = false;

    // Contador utilizado para limitar la duración del salto y el decremento de velocidad en este
    float jumpCounter = 0;
    
    // Valor que indica en qué dirección se está moviendo el jugador (1, -1) o si no se está moviendo (0)
    float movement;
    
    // Velocidad de movimiento horizontal a alcanzar (para un movimiento más suave), y la velocidad horizontal actualmente
    float proxSpeed, hspeed = 0;

    // Velocidad máxima de movimiento horizontal y salto vertical
    float speed = 10, jumpMax = 15;

    // Entero de acceso público que indica hacia qué dirección mira el jugador (usado para determinar si invertir/girar el sprite
    // y por los Boos para determinar si están siendo observados o no)
    public int face = 1;

    // Texto del canvas que indica cuántas monedas tiene el jugador
    public TextMeshProUGUI coinText;

    // Imagen del canvas que indica los puntos de salud actuales (0 a 4)
    public Image healthHUD;

    // Cuál imagen mostrará lo anterior según los puntos de vida, por cada valor posible entre cero y cuatro hay una imagen configurada en 
    // un vector desde el propio prefab
    public Sprite[] healthSpr;

    // Cuántas monedas tiene el jugador en el instante actual, si llega a ser 100 o más, los puntos de salud se ponen a cuatro (valor máximo)
    // y el número de monedas se resetea a cero.
    public static int Coins = 0;

    // Los puntos de salud actuales
    int HP = 4;

    // Inicialización de las variables
    void Start()
    {
        rb              = GetComponent<Rigidbody2D>();
        capsule         = GetComponent<CapsuleCollider2D>();
        animator        = GetComponent<Animator>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        spotLight       = GetComponent<Light2D>();
        audioSource     = GetComponent<AudioSource>();

        normalOffset    = capsule.offset;
        crouchOffset    = capsule.offset;
        crouchOffset.y  = 0.7f;

        normalSize      = capsule.size;
        crouchSize      = capsule.size;
        crouchSize.y    = capsule.size.y/2;

        // Empezar la corrutina que corresponde a la transición de inicio de nivel
        StartCoroutine(Begin());
    }

    void Update() {

        // Cambiar la animación/sprite del jugador dependiendo del estado actual de este:

        // Estos valores determinan si el jugador está saltando o corriendo
        animator.SetFloat("vspeed", rb.linearVelocityY);
        animator.SetFloat("hspeed", Mathf.Abs(rb.linearVelocityX));

        // Determinar si el jugador está tocando suelo o saltando (moviéndose hacia arriba)
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("jump", isJumping);

        // Determinar si, cuando se está corriendo, se hace un cambio repentino de dirección
        animator.SetBool("chdir", changeDirection);

        // Determinar si el jugador está agachado
        animator.SetBool("crouch", isCrouching);

        // Setear la imagen de la interfaz correspondiente a la salud actual
        healthHUD.sprite = healthSpr[HP];

        // Si ya se terminó el nivel, el jugador dejará de moverse y mirará hacia la izquierda
        if (levelEnd) {
            proxSpeed = 0;
            spriteRenderer.flipX = false;
            return;
        }

        if(Coins >= 100) {
            // Poner la salud al máximo una vez se consigan cien monedas
            HP = 4;
            // Poner las monedas a cero
            Coins = 0;
        }

        // Si el jugador está agachado, detener el movimiento horizontal
        if(isCrouching && isGrounded) 
            proxSpeed = 0;
        // Si el jugador no está agachado, la velocidad horizontal a la que se aproximará dependerá de la dirección y la velocidad máxima actual
        else 
            proxSpeed = movement * speed; 

        // Si el jugador está arrimado hacia la pared antes de un walljump, la dirección del sprite será inversa, se utiliza el sprite de cambio de dirección,
        // dando la ilusión de que Mario se apoya contra la pared antes de saltar
        if(Mathf.Abs(hspeed) > 0.1 && !canWallJump)
            spriteRenderer.flipX = changeDirection?(hspeed < 0):(hspeed >= 0);
        else if (canWallJump)
            spriteRenderer.flipX = (face < 0);


        // Determinar si se está cambiando de dirección mientras se corre
        changeDirection = proxSpeed * hspeed < 0 && !isCrouching;

        coinText.text = "x " + Coins;
        
    }

    void FixedUpdate()
    {

        // Animación del salto que se da cuando el jugador muere... 
        if(dead) {
            rb.linearVelocityY =  jumpMax-jumpCounter;
            if(jumpCounter<=jumpMax*2) jumpCounter += 0.4f;
            // ...deshabilitando el resto de la física del jugador
            return;
        }

        // Si se está saltando, moverse hacia arriba, cada vez menos para dar la ilusión de saltar,
        // no queremos que siempre que se presione el botón de saltar el jugador se esté elevando constantemente hacia arriba.
        if(isJumping) {
            rb.linearVelocityY =  jumpMax-jumpCounter;
            jumpCounter += 0.4f;
            // Una vez que se agote la energía de salto, se cae hacia abajo y se deja de saltar
            if(jumpCounter>=jumpMax) isJumping = false;
        } 

        // Si el jugador está arrimado a la pared (antes de hacer un walljump) y no está tocando suelo, 
        // se detiene el movimiento horizontal y se activa la animación de walljump (equivalente a la de cambiar dirección pero invertida en el eje horizontal)
        if(rb.linearVelocityX != hspeed && rb.linearVelocityX == 0 && !canWallJump && !isCrouching) {
            hspeed = 0;
            canWallJump = Mathf.Abs(rb.linearVelocityY) > 0.1;
            if(canWallJump) animator.SetTrigger("walljump");
        }

        // Aproximar la velocidad actual a proxSpeed para suavizar el movimiento horizontal

                if(proxSpeed > rb.linearVelocityX)  hspeed += 0.5f;
        else    if(proxSpeed < rb.linearVelocityX)  hspeed -= 0.5f;

        rb.linearVelocityX = hspeed;


        // Si se ha caído por debajo del nivel, Mario muere
        if (rb.transform.position.y < zone.transform.position.y - zone.GetComponent<BoxCollider2D>().bounds.size.y/2 - capsule.bounds.size.y) 
            StartCoroutine(Death());
    }

    // Cuando se colisiona con el suelo...
    void collideWithFloor(Collision2D collision) {

        // Mario está tocando suelo
        isGrounded = true; 

        foreach (ContactPoint2D point in collision.contacts) {

            // Si mario está golpeando bloques de suelo (techo) con la cabeza al saltar
            if(point.point.y >= rb.transform.position.y + capsule.size.y * 0.8)
                // Se detiene el salto, así se evita que se quede pegado al techo mientras se agote el contador de salto
                isJumping = false;

            // Si toca suelo, no se podrá hacer walljump
            if(point.point.y >= rb.transform.position.y) 
                canWallJump = false;
        }

    } 

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.layer) {

            // Manejar las colisiones con Powerups
            case 11:

                // Obtener el tipo de Powerup
                int type = collision.gameObject.GetComponent<Powerup>().getType();

                switch(type) {

                    // Si es el champiñón amarillo, añadir un punto de salud/vida (máximo 4)
                    case 1:
                        if(HP < 4) ++HP;
                    break;

                    // Si es el champiñón morado, recibir daño
                    case 2:
                        takeDamage();
                    break;

                }

                // Destruir el objeto
                Destroy(collision.gameObject);

            break;

            // Manejar las colisiones con bloques (qBlock / bBlock)
            case 10:

                // Collider del bloque
                BoxCollider2D bc =  collision.gameObject.GetComponent<BoxCollider2D>();

                // Detectar si se ha golpeado el bloque por debajo mientras se salta
                float yHitLine =  bc.transform.position.y - bc.bounds.size.y - capsule.size.y + 0.1f;
                if (isJumping && rb.transform.position.y > yHitLine)
                        collision.gameObject.GetComponent<Block>().hit();

                // Colisionar con este
                collideWithFloor(collision);

                break;

            // Si se colisiona con el suelo normal, sólamente colisionar con este
            // Esto también se aplica a los pinchos:
            //
            //      ·   Todo lo que está en la capa harm son: 
            //          ·   Sólidos que hacen daño en el contacto (si tienen Rigidbody)
            //              Después se colisiona con esto como si fuera suelo normal, los pinchos no se atraviesan
            //          ·   Objetos o enemigos (a los que no se pueden matar) atravesables (si no tienen rigidbody y su collider está puesto a trigger)
            //              Los Boos entran en esta categoría

            case 3:
                collideWithFloor(collision);
                break;

            case 12:
                collideWithFloor(collision);
                break;
        }
    }

    // Cuando se recibe daño...
    void takeDamage() {

        // Para evitar recibir daño varios fotogramas seguidos, cada vez que ocurre esto, el jugador se queda en un estado temporal en el que
        // no puede recibir daño, donde el sprite se vuelve transparente y rojizo para indicar que el daño se ha recibido, antes de volver al estado anterior 
        // Durante este periodo, canGetHurt es falso

        if (!canGetHurt) return;

        // Si sólamente queda un punto de vida, el jugador muere
        if(HP <= 1) {
            StartCoroutine(Death());
            return;
        }

        // Si no, se entra en el estado mencionado anteriormente, perdiendo un punto de vida
        StartCoroutine(Hurt());
    }

    // La corrutina de muerte
    IEnumerator Death() {

        // Se pierde todos los puntos de vida
        HP = 0;

        // Desactivar salto manual
        jumpCounter = 0;

        // Se cambia el sprite al de Mario muerto
        animator.SetTrigger("dead");

        // Desactivar colisiones
        rb.bodyType = RigidbodyType2D.Kinematic;
        capsule.enabled = false;

        // Detener todo movimiento
        rb.linearVelocity = Vector2.zero;

        // El jugador está muerto
        dead = true;

        // Se lanza la corrutina de transición de final del nivel
        StartCoroutine(End());
        yield break;
    }

    // Corrutina de recibir daño
    IEnumerator Hurt() {

        // Durante este periodo no se podrá recibir más daño
        canGetHurt = false;

        // Se resta un punto de vida
        --HP;

        // El color y transparencia del sprite van parpadeando para indicar tanto el daño recibido como un tiempo de invencibilidad para escapar del peligro
        for(int i=0; i<10; ++i) {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, (i%2==0)?0.9f:0.5f);
            yield return new WaitForSeconds(0.1f);
        }

        // El color del jugador vuelve a la normalidad
        spriteRenderer.color = Color.white;

        // Se puede volver a recibir daño
        canGetHurt = true;
    }

    // Cuando se termina el nivel (al pasar más allá de la bandera)
    public void endLevel() {
        // Se lanza la corrutina de final de nivel, el booleano dead es clave, este indica si volver a empezar el nivel si Mario ha muerto
        dead = false;
        StartCoroutine(End());
    }


    // Corrutina de final de nivel, encargada de hacer el efecto de "spotlight" al terminar un nivel
    IEnumerator End()
    {

        // Se activa el efecto de luz
        spotLight.enabled = true;

        // Duración de la transición y el tiempo recorrido de esta
        float duration = 1f; 
        float elapsedTime = 0f;

        // Radios iniciales y finales de la luz, para que no sea demasiado suave ni de baja calidad (si los radios están muy juntos, se pixela el efecto)

        float initialInnerRadius = 40f;
        float targetInnerRadius = 0f;

        float initialOuterRadius = 41f;
        float targetOuterRadius = 0.01f;

        // Disminuir el radio
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            spotLight.pointLightInnerRadius = Mathf.Lerp(initialInnerRadius, targetInnerRadius, t);
            spotLight.pointLightOuterRadius = Mathf.Lerp(initialOuterRadius, targetOuterRadius, t);

            yield return null;
        }


        // Se espera un poco antes de cargar la siguiente escena
        yield return new WaitForSeconds(1f);

        // Si se ha muerto previamente, se repite el nivel, si no, se va al siguiente
        SceneManager.LoadScene(dead? SceneManager.GetActiveScene().name : nextScene);
    }


    // Corrutina de inicio del nivel, esta es equivalente a la anterior, pero empieza con la pantalla a negro y va aumentando los radios
    IEnumerator Begin() {

        // Para evitar que todo se vea negro en el editor, la luz está desactivada siempre, sólo se activa dentro de los efectos
        spotLight.enabled = true;

        // Duración de la transición y el tiempo recorrido 
        float duration = 1f; 
        float elapsedTime = 0f;

        // Radios iniciales y finales de la luz

        float initialInnerRadius = 0f;
        float targetInnerRadius = 40f;

        float initialOuterRadius = 0.01f;
        float targetOuterRadius = 41f;

        // Aumentar el radio
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            spotLight.pointLightInnerRadius = Mathf.Lerp(initialInnerRadius, targetInnerRadius, t);
            spotLight.pointLightOuterRadius = Mathf.Lerp(initialOuterRadius, targetOuterRadius, t);

            yield return null;
        }

        // Para evitar un efecto de vignette indeseado, desactivar el efecto de la luz
        spotLight.enabled = false;

    }

    // Manejar colisiones con triggers
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el trigger es de una moneda, se añade una moneda al contador y esta desaparece
        switch(collision.gameObject.layer){
            case 7:
                Destroy(collision.gameObject);
                ++Coins;
            break;
        }
    }

    // Manejar colisiones con el boo, si se mantiene dentro de este, se va recibiendo daño
    // Si usáramos OnTriggerEnter, sólo se perdería un punto de vida al colisionar con este
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 12)
            takeDamage();
    }

    // Lo mismo pasa con los pinchos, queremos que se reciba daño siempre que se esté colisionando, no una sola vez
    void OnCollisionStay2D(Collision2D collision)
    {
        switch(collision.gameObject.layer) {

            case 12:
            takeDamage();
                    // En caso de enemigos a los que se puedan pisar/matar
            break;

            case 9:

                // Referencias al rigidbody y collider del enemigo
                Rigidbody2D erb = collision.gameObject.GetComponent<Rigidbody2D>();
                CapsuleCollider2D ebc =  collision.gameObject.GetComponent<CapsuleCollider2D>();

                // Componente del script Enemy.cs
                Enemy e = collision.gameObject.GetComponent<Enemy>();

                // Detectar si lo ha pisado por arriba
                float yDamageLine =  erb.transform.position.y - ebc.size.y * 0.75f;
                if (!isGrounded && rb.transform.position.y > yDamageLine) {

                    // El enemigo se mata
                    e.kill();

                    // El jugador "rebota" (en realidad salta de nuevo)
                    isJumping = true;

                    // Se termina este segmento de código
                    break;
                }

                // Si no se ha pisado (se ha colisionado por el lado), recibir daño
                takeDamage();

            break;
        }
    }

    // Si salimos de la colisión con el suelo o capas sólidas del "grid"...
    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3 || collision.gameObject.layer == 10 || collision.gameObject.layer == 12) 
            // El jugador no estará tocando suelo
            isGrounded = false;    
    }


    // Funciones para comunicarse con el nuevo sistema de input de Unity

    // Si se detecta movimiento (izquierda/derecha)...
    public void Move(InputAction.CallbackContext context) {

        // Si ya se terminó el nivel, no hacemos nada
        if(levelEnd) return;

        // Detectar si se mueve a la izquierda o a la derecha
        movement = context.ReadValue<Vector2>().x;
        if(movement != 0)
            face = (int)(movement/Math.Abs(movement));
    }

    // Si se detecta el botón de salto
    public void Jump(InputAction.CallbackContext context) {

        // Si ya se terminó el nivel, no hacemos nada
        if(levelEnd) return;

        // Si el jugador está tocando suelo y se presiona el botón
        if(isGrounded && context.performed) {

            // El jugador está saltando
            isJumping = true;

            // Si se está arrimado a la pared sin tocar suelo con los pies, se realiza un "walljump"
            if(canWallJump) 
                hspeed = -face * speed;
            
            // Poner a cero el contador de salto
            jumpCounter = 0;
            
        } 
        
        // Si se deja de presionar el botón, se deja de saltar, de esta forma, cuanto menos tiempo se pulse el botón, menos se salta
        if(context.canceled) isJumping = false;
    }

    // Si se detecta el botón de correr
    public void Sprint(InputAction.CallbackContext context) {

        // Si ya se terminó el nivel, no hacemos nada
        if(levelEnd) return;

        // Si se presiona, se duplica la velocidad, si se deja de presionar, la velocidad vuelve a ser la normal
        if(context.performed) 
            speed = 20;
        else if(context.canceled)
            speed = 10;

    }

    // Si se detecta el botón de agacharse
    public void Crouch(InputAction.CallbackContext context) {

        // Si ya se terminó el nivel, no hacemos nada
        if(levelEnd) return;

        // Si se presiona el botón
        if(context.performed) {

            // Si se está tocando suelo
            if(isGrounded) {
                // El jugador está agachado
                isCrouching = true;
                // Se cambia el tamaño del collider y el offset para evitar movimientos repentinos indeseados
                capsule.size = crouchSize;
                capsule.offset = crouchOffset;
            }

            // No se puede hacer walljump cuando se está agachado
            canWallJump = false;

        // Si se deja de presionar
        }else if(context.canceled) {

            //El jugador ya no se está agachando
            isCrouching = false;

            // El tamaño y offset del collider vuelven a la normalidad
            capsule.size = normalSize;
            capsule.offset = normalOffset;
        }
    }
}
