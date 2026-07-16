using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Counts up "!" -> "!!" -> "!!!" in black during the lead-up to the worker
// turning, as a warning. That alone never turns red - red "[!]" only shows
// if the worker's turn actually catches the player mid-rip (triggered by
// PosterRipMiniGame, not by this countdown finishing).
public class ExclamationCountdown : MonoBehaviour
{
    public Text text;
    public Color warningColor = Color.black;
    public Color dangerColor = Color.red;

    Coroutine activeRoutine;

    // Steps through "!" -> "!!" -> "!!!" over `duration` seconds.
    public void PlayCountdown(float duration)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        // Must activate before StartCoroutine - Unity refuses to start a
        // coroutine on an inactive GameObject, silently doing nothing.
        gameObject.SetActive(true);
        activeRoutine = StartCoroutine(CountdownRoutine(duration));
    }

    IEnumerator CountdownRoutine(float duration)
    {
        if (text != null) text.color = warningColor;

        string[] stages = { "!", "!!", "!!!" };
        float stepTime = duration / stages.Length;
        foreach (string stage in stages)
        {
            if (text != null) text.text = stage;
            yield return new WaitForSeconds(stepTime);
        }

        gameObject.SetActive(false);
    }

    // Shown only when the worker actually notices the player mid-rip.
    public void ShowDanger()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
        gameObject.SetActive(true);
        if (text != null)
        {
            text.color = dangerColor;
            text.text = "[!]";
        }
    }

    public void Hide()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
        gameObject.SetActive(false);
    }
}
