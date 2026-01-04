using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // FADE TO #########################################################
    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }
    
    IEnumerator FadeIn()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime; // Use unscaledDeltaTime so fade works even when paused
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }

    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime; // Use unscaledDeltaTime so fade works even when paused
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        Time.timeScale = 1f; // Ensure time scale is reset before loading scene
        SceneManager.LoadScene(scene);
    }

    // Called when a new scene is loaded - ensures lighting is properly initialized
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset time scale when scene loads (in case it was paused)
        Time.timeScale = 1f;
        // Force Unity to update lighting for the new scene
        // This ensures lighting settings from the scene are properly applied
        StartCoroutine(UpdateLightingNextFrame());
    }

    IEnumerator UpdateLightingNextFrame()
    {
        yield return null; // Wait one frame for scene to fully initialize
        // Force lighting update
        DynamicGI.UpdateEnvironment();
    }
}
