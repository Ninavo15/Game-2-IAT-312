using UnityEngine;
using UnityEngine.Timeline;

public class GhostTile : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scareSound;
    public VignetteEffect vignetteEffect;
    [SerializeField] CarMovement car;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioSource.PlayOneShot(scareSound);
            vignetteEffect.TriggerVignette();
            car.stopped = true;

        }

    }
}
