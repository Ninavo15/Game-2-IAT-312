using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class GhostTile : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scareSound;

    [Header("Scripts")]
    [SerializeField] promptPop pp;
    public VignetteEffect vignetteEffect;
    [SerializeField] CarMovement car;

    [Header("GameObject")]
    [SerializeField] GameObject ghost;
    [SerializeField] GameObject carr;
    [SerializeField] GameObject pov;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject keys;
    [SerializeField] GameObject slot;


    public float ghostDuration = 2.0f;
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioSource.PlayOneShot(scareSound);
            vignetteEffect.TriggerVignette();
            car.stopped = true;
            StartCoroutine(GhostSequence());

            pp.ShowPrompt(pp.promptText1);
            StressSystem.AddPoint(1);
            Debug.Log("stress point: " + StressSystem.stressPoints);
        }

    }

    IEnumerator GhostSequence()
    {
        yield return new WaitForSeconds(ghostDuration);
        slot.SetActive(true);
        ghost.SetActive(false);
        carr.SetActive(false);
        pov.SetActive(true);
        heart.SetActive(true);
        keys.SetActive(true);
        
    }
}
