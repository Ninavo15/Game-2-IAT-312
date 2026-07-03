using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalChoiceController : MonoBehaviour
{
    [Header("Siren")]
    public AudioSource sirenAudioSource;
    public AudioClip sirenClip;
    public float sirenFadeInDuration = 4f;
    public float sirenMaxVolume = 1f;

    [Header("Dialogue")]
    public DialogueController dialogue;
    public float dialogueDelay = 1f;

    [Header("Choice UI")]
    public GameObject choicePanel;

    [Header("Scenes")]
    public string runAwayScene = "Scene 4";
    public string turnInScene = "Ending";

    bool triggered = false;

    public void TriggerFinalChoice()
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(FinalChoiceSequence());
    }

    IEnumerator FinalChoiceSequence()
    {
        if (sirenAudioSource != null && sirenClip != null)
        {
            sirenAudioSource.clip = sirenClip;
            sirenAudioSource.loop = true;
            sirenAudioSource.volume = 0f;
            sirenAudioSource.Play();
            StartCoroutine(FadeSirenVolume());
        }

        if (dialogue != null) dialogue.ShowLine("Oh No! Police is coming!!!", 3f);

        yield return new WaitForSeconds(3f + dialogueDelay);

        if (dialogue != null) dialogue.ShowLine("What should I do?", 3f);

        yield return new WaitForSeconds(2f);

        if (choicePanel != null) choicePanel.SetActive(true);
    }

    IEnumerator FadeSirenVolume()
    {
        float t = 0f;
        while (t < sirenFadeInDuration)
        {
            t += Time.deltaTime;
            sirenAudioSource.volume = Mathf.Lerp(0f, sirenMaxVolume, t / sirenFadeInDuration);
            yield return null;
        }
        sirenAudioSource.volume = sirenMaxVolume;
    }

    public void RunAway()
    {
        SceneManager.LoadScene(runAwayScene);
    }

    public void TurnYourselfIn()
    {
        SceneManager.LoadScene(turnInScene);
    }
}
