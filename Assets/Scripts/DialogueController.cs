using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static bool IsActive { get; private set; }

    public GameObject dialoguePanel;
    public Text dialogueText;
    public float displayDuration = 2.5f;
    public float charactersPerSecond = 30f;

    [Header("Intro (optional)")]
    [Tooltip("If set, this line plays automatically as soon as the scene starts.")]
    public string introLine;
    public float introDelay = 0.5f;

    [Header("Entrance sound (optional)")]
    [Tooltip("Played immediately when the scene starts, before introLine appears. Skipped along with introLine on a return trip (see skipIntro).")]
    public AudioSource entranceAudio;
    public AudioClip entranceClip;

    Coroutine current;
    bool skipIntro;

    void Awake()
    {
        IsActive = false; // fresh scene, fresh state - never trust leftovers from the last scene

        // Returning to Scene 4 from the item-selection mini-game reloads this same
        // scene, which would otherwise replay the "need to buy" intro line every
        // time. Read the flag here - Awake runs for every object in the scene
        // before any Start does - so it doesn't race whichever script consumes it.
        skipIntro = GlobalStore.returnedFromSelection;
    }

    void Start()
    {
        if (skipIntro) return;

        if (entranceAudio != null && entranceClip != null) entranceAudio.PlayOneShot(entranceClip);

        if (!string.IsNullOrEmpty(introLine)) StartCoroutine(ShowIntroAfterDelay());
    }

    IEnumerator ShowIntroAfterDelay()
    {
        // Block movement/interaction (both gated on IsActive) from the moment
        // the scene starts, not just once the line begins revealing - otherwise
        // the player can walk around during introDelay before the line appears.
        IsActive = true;
        if (introDelay > 0f) yield return new WaitForSeconds(introDelay);
        ShowLine(introLine);
    }

    // Interactable prompt in every future scene. Force it clear here instead.
    void OnDestroy()
    {
        IsActive = false;
    }

    public void ShowLine(string line)
    {
        ShowLine(line, displayDuration);
    }

    public void ShowLine(string line, float duration)
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(ShowLineRoutine(line, duration));
    }

    IEnumerator ShowLineRoutine(string line, float duration)
    {
        IsActive = true;
        dialoguePanel.SetActive(true);

        // Reveal the line left to right
        dialogueText.text = "";
        float delayPerChar = 1f / Mathf.Max(charactersPerSecond, 1f);
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(delayPerChar);
        }

        yield return new WaitForSeconds(duration);
        dialoguePanel.SetActive(false);
        IsActive = false;
    }
}
