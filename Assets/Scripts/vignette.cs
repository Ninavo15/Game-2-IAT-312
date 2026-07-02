using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteEffect : MonoBehaviour
{
    public Volume volume;
    public float fadeInDuration = 0.5f;
    public float holdDuration = 3.0f;
    public float fadeOutDuration = 3.0f;
    public float maxIntensity = 0.8f;

    private Vignette vignette;

    void Start()
    {
        volume.profile.TryGet(out vignette);
    }

    public void TriggerVignette()
    {
        StartCoroutine(VignetteSequence());
    }

    IEnumerator VignetteSequence()
    {
        yield return StartCoroutine(FadeVignette(0f, maxIntensity, fadeInDuration));
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(FadeVignette(maxIntensity, 0f, fadeOutDuration));
    }

    IEnumerator FadeVignette(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = to;
    }
}