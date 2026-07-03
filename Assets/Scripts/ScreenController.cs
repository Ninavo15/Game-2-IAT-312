using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour
{
    enum State { Off, MissingScreen, Footage }

    [Header("Monitor")]
    public SpriteRenderer screenRenderer;
    public Sprite missingScreenSprite;
    public Sprite footageSprite;

    [Header("POV")]
    public GameObject povPanel;
    public Image povImage;
    public AudioSource povAudioSource;
    public AudioClip footageAudio;
    public float missingDisplayDuration = 1.5f;
    public CharacterMovement character;
    public Interactable screenInteractable;
    public Interactable carInteractable;
    public Text carPromptText;

    [Header("POV Image Scale")]
    [Tooltip("Relative on-screen scale of the missing poster inside the POV frame.")]
    public float missingImageScale = 1f;
    [Tooltip("Relative on-screen scale of the footage clip inside the POV frame. Lower this if footage looks bigger than the missing poster.")]
    public float footageImageScale = 0.9f;

    [Header("Dialogue")]
    public DialogueController dialogue;
    public FinalChoiceController finalChoice;

    State state = State.Off;
    bool sequenceComplete = false;
    bool corpseDialogueShown = false;

    public void OnCarInteract()
    {
        if (state == State.Off)
        {
            state = State.MissingScreen;
            screenRenderer.sprite = missingScreenSprite;
            if (dialogue != null) dialogue.ShowLine("Why is there a blood stain here?");

            // The blood stain has been checked — no re-checking until the
            // screen sequence finishes and re-enables this for "check the car".
            if (carInteractable != null) carInteractable.enabled = false;
        }
        else if (sequenceComplete && !corpseDialogueShown)
        {
            corpseDialogueShown = true;

            // The car has been fully checked — lock out further interaction.
            if (carInteractable != null) carInteractable.enabled = false;

            StartCoroutine(ShowCorpseDialogueThenChoice());
        }
    }

    IEnumerator ShowCorpseDialogueThenChoice()
    {
        const float corpseDuration = 5f;
        if (dialogue != null) dialogue.ShowLine("The corpse of the girl is in my car trunk.", corpseDuration);
        yield return new WaitForSeconds(corpseDuration);
        if (finalChoice != null) finalChoice.TriggerFinalChoice();
    }

    public void ShowFootage()
    {
        if (state != State.MissingScreen) return;
        state = State.Footage;
        screenRenderer.sprite = footageSprite;
        StartCoroutine(PlayPOVSequence());
    }

    IEnumerator PlayPOVSequence()
    {
        if (character != null) character.enabled = false;

        povPanel.SetActive(true);
        povImage.sprite = missingScreenSprite;
        povImage.rectTransform.localScale = Vector3.one * missingImageScale;
        StartCoroutine(ShowLineDelayed("This girl looks familiar...", 2f));

        // Audio starts as soon as the missing poster is shown.
        float audioLength = 0f;
        if (povAudioSource != null && footageAudio != null)
        {
            povAudioSource.clip = footageAudio;
            povAudioSource.Play();
            audioLength = footageAudio.length;
        }

        yield return new WaitForSeconds(missingDisplayDuration);

        povImage.sprite = footageSprite;
        povImage.rectTransform.localScale = Vector3.one * footageImageScale;
        StartCoroutine(ShowLineDelayed("Wait! That's my car!!!", 2f));

        // Stay open for whatever's left of the audio so both finish together.
        float remaining = audioLength - missingDisplayDuration;
        if (remaining > 0f) yield return new WaitForSeconds(remaining);

        povPanel.SetActive(false);
        if (character != null) character.enabled = true;

        // After The screen has been fully watched,lock out further interaction.
        if (screenInteractable != null) screenInteractable.enabled = false;

        //update the car's prompt.
        if (carPromptText != null) carPromptText.text = "Press E to check the car";

        // Re-enable the car now that there's something new to check.
        if (carInteractable != null) carInteractable.enabled = true;

        sequenceComplete = true;
        if (dialogue != null) dialogue.ShowLine("I need to check my car.");
    }

    IEnumerator ShowLineDelayed(string line, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialogue != null) dialogue.ShowLine(line);
    }
}
