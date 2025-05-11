using System;
using UnityEngine;

/*
    El siguiente código se encarga del efecto "parallax" del fondo, dando la ilusión de profundidad

    Este script, del prefab "background", se encarga de mover las tres capas (cada una compuesta por tres imágenes seguidas del mismo asset)
    con respecto a la cámara.

    En el juego original de "New Super Mario Bros", además de esto, las nubes tienen su propio movimiento, así que, usando gimp, he separado las nubes de las otras
    capas, para además darles movimiento constante (incluso si el jugador o la cámara están parados), dando la ilusión de viento.

    Este efecto, si aplicado a todas las capas, da la sensación de que la cámara se está moviendo constantemente, esto es utilizado en las pantallas de inicio
    y final del juego
*/

public class BackgroundController : MonoBehaviour
{

    // Longitud de la imagen de fondo
    float length;

    // Vector de posiciones de inicio en la coordenada x para cada capa
    float[] startPos = {0,0,0,0};

    // Vector de posiciones de cada capa
    Vector3[] finalPos = {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

    // Cámara 
    public GameObject cam, reference;

    // Capas
    public GameObject[] layers;

    // Movimiento extra (viento) por cada capa
    public float[] windPerLayer;

    // Posición "offset" de cada capa con respecto al viento
    float[] windOffset = {0,0,0,0};

    void Start()
    {

        // Se inicializan las posiciones x iniciales y los vectores de posición de cada capa...

        for(int i=0; i<1; ++i) {
            finalPos[i] = layers[i].transform.position;
            startPos[i] = layers[i].transform.position.x;
        }

        // ... Junto a la longitud de cada imagen individual de esta

        length = reference.GetComponent<Renderer>().bounds.size.x;
    }

    // Actualizar posiciones en cada fotograma
    // Basado en el siguiente tutorial: https://www.youtube.com/watch?v=AoRBZh6HvIk

    void FixedUpdate()
    {
        for (int i=0; i<4; ++i) {
            float parallaxEffect =  0.1f * (i+1);
            float distance = cam.transform.position.x * (1 - parallaxEffect);
            float movement = cam.transform.position.x * parallaxEffect;

            if(movement > startPos[i] + length - windOffset[i]) 
                startPos[i] += length;
            if(movement < startPos[i] + length - windOffset[i])
                startPos[i] -= length;

            finalPos[i].x = startPos[i] + distance - windOffset[i];
            layers[i].transform.position = finalPos[i];

            windOffset[i] += windPerLayer[i];
            if(windOffset[i] >= length/2) 
                windOffset[i] = 0;
        }
    }
}
