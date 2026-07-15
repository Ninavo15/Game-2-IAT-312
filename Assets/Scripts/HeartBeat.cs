using System.Collections;
using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    public float beatSize = 1.15f;
    public float beatSpeed = 1f;

    Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(Beat());
    }

    IEnumerator Beat()
    {
        while (true)
        {
            // First beat
            transform.localScale = originalScale * beatSize;
            yield return new WaitForSeconds(beatSpeed);

            transform.localScale = originalScale;
            yield return new WaitForSeconds(0.3f);

            // Second beat
            transform.localScale = originalScale * (beatSize * 0.95f);
            yield return new WaitForSeconds(beatSpeed);

            transform.localScale = originalScale;

            // Pause 
            yield return new WaitForSeconds(0.6f);
        }
    }
}