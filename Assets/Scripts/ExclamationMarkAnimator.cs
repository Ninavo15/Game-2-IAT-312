using System.Collections;
using UnityEngine;

// Pops the exclamation mark in with a quick scale punch, then keeps it
// blinking on/off like a warning light for as long as it's shown.
[RequireComponent(typeof(SpriteRenderer))]
public class ExclamationMarkAnimator : MonoBehaviour
{
    public float popDuration = 0.15f;
    public float popScale = 1.4f;
    public float blinkInterval = 0.25f;

    SpriteRenderer spriteRenderer;
    Vector3 baseScale;
    Coroutine loopRoutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        spriteRenderer.enabled = true;
        transform.localScale = baseScale;
        if (loopRoutine != null) StopCoroutine(loopRoutine);
        loopRoutine = StartCoroutine(PopThenBlinkLoop());
    }

    public void Hide()
    {
        if (loopRoutine != null) StopCoroutine(loopRoutine);
        loopRoutine = null;
        transform.localScale = baseScale;
        gameObject.SetActive(false);
    }

    IEnumerator PopThenBlinkLoop()
    {
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(popScale, 1f, t / popDuration);
            transform.localScale = baseScale * scale;
            yield return null;
        }
        transform.localScale = baseScale;

        bool visible = true;
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            visible = !visible;
            spriteRenderer.enabled = visible;
        }
    }
}
