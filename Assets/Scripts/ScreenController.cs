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
    public Text carPromptText;

    [Header("POV Image Scale")]
    [Tooltip("Relative on-screen scale of the missing poster inside the POV frame.")]
    public float missingImageScale = 1f;
    [Tooltip("Relative on-screen scale of the footage clip inside the POV frame. Lower this if footage looks bigger than the missing poster.")]
    public float footageImageScale = 0.9f;

    State state = State.Off;

    public void TurnOnAndShowMissing()
    {
        if (state != State.Off) return;
        state = State.MissingScreen;
        screenRenderer.sprite = missingScreenSprite;
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

        // Stay open for whatever's left of the audio so both finish together.
        float remaining = audioLength - missingDisplayDuration;
        if (remaining > 0f) yield return new WaitForSeconds(remaining);

        povPanel.SetActive(false);
        if (character != null) character.enabled = true;

        // The screen has been fully watched — lock out further interaction.
        if (screenInteractable != null) screenInteractable.enabled = false;

        // The blood stain has already been found — update the car's prompt.
        if (carPromptText != null) carPromptText.text = "Press E to check the car";
    }
}
