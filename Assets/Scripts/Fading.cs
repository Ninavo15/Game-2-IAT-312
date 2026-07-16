using System.Collections;
using UnityEngine;

public class Fading : MonoBehaviour
{
    [SerializeField] CanvasGroup canGroup;
    public float fadeDuration = 3.0f;
    [SerializeField] private bool fadeIn = false;

    // True while a fade is in progress, so other scripts (e.g. player
    // movement) can pause themselves during scene transitions.
    public static bool IsFading { get; private set; }

    private void Awake()
    {
        IsFading = false; // fresh scene, fresh state - never trust leftovers from the last scene
    }

    private void Start()
    {
        if (fadeIn)
        {
            FadeIn();
        } else {
            FadeOut();
        }

    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canGroup, canGroup.alpha, 0, fadeDuration));
    }
    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canGroup, canGroup.alpha, 1, fadeDuration));
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        IsFading = true;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
        IsFading = false;
    }
}
