using System;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    float length;
    float[] startPos = {0,0,0,0};
    Vector3[] finalPos = {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
    public GameObject cam, reference;
    public GameObject[] layers;
    public float[] windPerLayer;
    float[] windOffset = {0,0,0,0};

    void Start()
    {
        for(int i=0; i<1; ++i) {
            finalPos[i] = layers[i].transform.position;
            startPos[i] = layers[i].transform.position.x;
        }

        length = reference.GetComponent<Renderer>().bounds.size.x;
    }


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
