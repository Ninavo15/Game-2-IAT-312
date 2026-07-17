using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip radioTurnOnClip;
    public AudioClip endingClip;
    public bool loopEndingClip = false;
    public Fading fade;

    [Header("Subtitles (optional)")]
    [Tooltip("Shown one at a time while endingClip plays, each held for a share of the clip's length proportional to its text length.")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] endingLines;

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
            if (endingLines != null && endingLines.Length > 0) StartCoroutine(PlaySubtitles(endingClip.length));
            yield return new WaitForSeconds(endingClip.length);
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        audioSource.Stop();

        if (fade != null) fade.FadeOut();
    }

    IEnumerator PlaySubtitles(float totalDuration)
    {
        if (dialoguePanel == null || dialogueText == null) yield break;

        int totalChars = 0;
        foreach (string line in endingLines) totalChars += line.Length;
        if (totalChars <= 0) yield break;

        dialoguePanel.SetActive(true);
        foreach (string line in endingLines)
        {
            dialogueText.text = line;
            float lineDuration = totalDuration * (line.Length / (float)totalChars);
            yield return new WaitForSeconds(lineDuration);
        }
        dialoguePanel.SetActive(false);
    }
}
