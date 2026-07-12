using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class GhostTile : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scareSound;
    public VignetteEffect vignetteEffect;
    [SerializeField] CarMovement car;
    [SerializeField] GameObject ghost;
    [SerializeField] promptPop pp;
    [SerializeField] GameObject carr;
    [SerializeField] GameObject pov;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject keys;


    public float ghostDuration = 2.0f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioSource.PlayOneShot(scareSound);
            vignetteEffect.TriggerVignette();
            car.stopped = true;
            StartCoroutine(GhostSequence());

            pp.ShowPrompt();
            StressSystem.AddPoint(1);
            Debug.Log("stress point: " + StressSystem.stressPoints);
        }

    }

    IEnumerator GhostSequence()
    {
        yield return new WaitForSeconds(ghostDuration);
        ghost.SetActive(false);
        carr.SetActive(false);
        pov.SetActive(true);
        heart.SetActive(true);
        keys.SetActive(true);
        
    }
}
