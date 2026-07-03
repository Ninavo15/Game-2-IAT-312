using System.Collections;
using UnityEngine;

public class EndingAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip radioTurnOnClip;
    public AudioClip endingClip;
    public bool loopEndingClip = false;
    public Fading fade;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        if (audioSource == null) yield break;

        if (radioTurnOnClip != null)
        {
            audioSource.loop = false;
            audioSource.clip = radioTurnOnClip;
            audioSource.Play();
            yield return new WaitForSeconds(radioTurnOnClip.length);
        }

        if (endingClip != null)
        {
            audioSource.loop = loopEndingClip;
            audioSource.clip = endingClip;
            audioSource.Play();
            yield return new WaitForSeconds(endingClip.length);
        }

        audioSource.Stop();

        if (fade != null) fade.FadeOut();
    }
}
