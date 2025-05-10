using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class titlescreen : MonoBehaviour
{
    
    public string nextlevel;
    Light2D spotLight;
    bool transition = true;

    IEnumerator End()
    {
        spotLight.enabled = true;

        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;

        // Store initial and target values
        float initialInnerRadius = 40f;
        float targetInnerRadius = 0f;

        float initialOuterRadius = 41f;
        float targetOuterRadius = 0.01f;

        // Smoothly transition over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            spotLight.pointLightInnerRadius = Mathf.Lerp(initialInnerRadius, targetInnerRadius, t);
            spotLight.pointLightOuterRadius = Mathf.Lerp(initialOuterRadius, targetOuterRadius, t);

            yield return null;
        }


        // Wait before loading the scene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextlevel);
    }

    IEnumerator Begin() {

        spotLight.enabled = true;

        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;

        // Store initial and target values
        float initialInnerRadius = 0f;
        float targetInnerRadius = 40f;

        float initialOuterRadius = 0.01f;
        float targetOuterRadius = 41f;

        // Smoothly transition over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            spotLight.pointLightInnerRadius = Mathf.Lerp(initialInnerRadius, targetInnerRadius, t);
            spotLight.pointLightOuterRadius = Mathf.Lerp(initialOuterRadius, targetOuterRadius, t);

            yield return null;
        }

        spotLight.enabled = false;
        transition = false;

    }

    void Start()
    {
        spotLight = GetComponent<Light2D>();
        StartCoroutine(Begin());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey && !transition) {
            Debug.Log("Starting...");
            transition = true;
            StartCoroutine(End());
        }
    }
}
