using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/*
    El siguiente código se ejecuta en las pantallas de inicio/final del juego
*/

public class titlescreen : MonoBehaviour
{
    
    // El nombre de la escena a la que se transiciona tras pulsar cualquier botón
    public string nextlevel;

    // Luz para los efectos de transición
    Light2D spotLight;

    // Para evitar disparar una transición mientras se hace otra, se usa este booleano
    bool transition = true;

    // Corrutina de transicionar a la siguiente escena
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
        SceneManager.LoadScene(nextlevel);
    }

    // Corrutina de inicio, esta es equivalente a la anterior, pero empieza con la pantalla a negro y va aumentando los radios
    IEnumerator Begin() {

        // Se activa el efecto de luz
        spotLight.enabled = true;

        // Duración de la transición y el tiempo recorrido de esta
        float duration = 1f; 
        float elapsedTime = 0f;

        // Radios iniciales y finales de la luz, para que no sea demasiado suave ni de baja calidad (si los radios están muy juntos, se pixela el efecto)

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

        // Se desactiva el efecto de luz y se indica que terminó la transición
        spotLight.enabled = false;
        transition = false;

    }

    void Start()
    {
        // Inicializar el componente spotLight;
        spotLight = GetComponent<Light2D>();

        // Lanzar la corrutina de inicio
        StartCoroutine(Begin());
    }

    void Update()
    {
        // Si se presiona cualquier botón y no se está transicionando
        if(Input.anyKey && !transition) {
            // Se inicia la transición y se indica que hay una transición en progreso (evitar disparar varias a la vez)
            transition = true;
            StartCoroutine(End());
        }
    }
}
