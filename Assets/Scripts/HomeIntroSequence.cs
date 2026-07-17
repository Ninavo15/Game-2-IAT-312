using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Horror scene that plays when the player turn on the light
public class HomeIntroSequence : MonoBehaviour
{
    [Header("Black overlay")]
    public Fading blackFade; // fades the initial full-black overlay out, and back in before the ending
    public CanvasGroup blackOverlayGroup; // used directly for the quick on/off flicker after the fade

    [Header("Light switch sound")]
    public AudioSource lightSwitchAudio;
    public AudioClip lightSwitchClip; // played every time the light flicks on or off

    [Header("TV sound")]
    public AudioSource tvAudio;
    public AudioClip tvNoiseClip; // looped for as long as the TV ghost segment runs

    [Header("Ghost appear sound")]
    public AudioSource ghostAppearAudio;
    public AudioClip ghostAppearClip; // played every time the room ghost or TV ghost pops into view

    [Header("Relief beat sound")]
    public AudioSource breathingAudio;
    public AudioClip breathingClip;
    public AudioSource heartbeatAudio;
    public AudioClip heartbeatClip;

    [Header("Jumpscare sound")]
    public AudioSource jumpscareAudio;
    public AudioClip jumpscareClip;
    public AudioSource screamAudio;
    public AudioClip screamClip;

    [Header("Dialogue")]
    public DialogueController dialogue;
    public string introLine = "I'm so tired. I need to find way to get rid all the evidences";
    public float introHoldDuration = 3f;
    public float introDelay = 0.5f;

    [Header("Light prompt")]
    public GameObject lightPrompt;

    [Header("Light flicker")]
    public string lightFlickerLine = "Huh!? What's happening???";
    public int lightFlickerCount = 3;
    public float lightFlickerStartInterval = 0.4f; // slow at first
    public float lightFlickerSpeedMultiplier = 0.6f; // interval shrinks by this each flicker - faster and faster

    [Header("Room ghost")]
    public GameObject roomGhost;
    // Glimpsed here (its original spot) during the light flicker, in sync with the light.
    public Vector2 roomGhostGlimpsePosition = new Vector2(-6.02f, -1.83f);
    public float roomGhostGlimpseScale = 4.627964f;

    [Header("TV ghost")]
    public GameObject tvGhost;
    public string tvGhostLine1 = "THAT'S GIRL!!! THE ONE I HIT";
    public int tvFlickerCount = 3;
    public float tvFlickerOnTime = 0.25f;
    public float tvFlickerOffTime = 0.2f;

    [Header("Room ghost - approach after TV")]
    // The ghost pops into view
    public string approachLine = "No! No! Stay Away!!!";
    public Vector2[] approachPositions = new Vector2[]
    {
        new Vector2(-6.02f, -1.83f),
        new Vector2(-4.38f, -2.49f),
        new Vector2(-0.16f, -3.97f),
    };
    public float[] approachScales = new float[] { 4.627964f, 6.5f, 9f };
    public float roomGhostOnTime = 0.3f;
    public float roomGhostOffTime = 0.2f;

    [Header("Relief beat")]
    public string reliefLine = "Is it end?";
    public float reliefHoldTime = 6f; // light on, no ghost in sight, before the jumpscare

    [Header("Final jumpscare")]
    public string jumpscareLine = "AAAAAAAA";
    public Vector2 jumpscarePosition = new Vector2(0.31f, -4.77f);
    public float jumpscareScale = 13.84275f;
    public float jumpscareHoldTime = 3f;

    [Header("Ending transition")]
    public string endingScene = "House Ending";

    void Start()
    {
        // blackOverlayGroup.alpha is already 1 (fully black) from the scene file.
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(introDelay);

        float revealTime = introLine.Length / Mathf.Max(dialogue.charactersPerSecond, 1f);
        dialogue.ShowLine(introLine, introHoldDuration);
        yield return new WaitForSeconds(revealTime + introHoldDuration);

        if (lightPrompt != null) lightPrompt.SetActive(true);
        yield return WaitForEPress();
        if (lightPrompt != null) lightPrompt.SetActive(false);

        // Fade the initial black overlay out normally...
        blackFade.FadeIn();
        yield return new WaitForSeconds(blackFade.fadeDuration);

        dialogue.ShowLine(lightFlickerLine, 2.5f);

        // ...then flicker it like a glitching light - slow at first, then
        // faster and faster - with the ghost glimpsed each time the light is on.
        float flickerInterval = lightFlickerStartInterval;
        for (int i = 0; i < lightFlickerCount; i++)
        {
            blackOverlayGroup.alpha = 1f;
            PlayLightSwitchSound();
            if (roomGhost != null) roomGhost.SetActive(false);
            yield return new WaitForSeconds(flickerInterval);

            blackOverlayGroup.alpha = 0f;
            PlayLightSwitchSound();
            if (roomGhost != null)
            {
                roomGhost.transform.position = roomGhostGlimpsePosition;
                roomGhost.transform.localScale = new Vector3(roomGhostGlimpseScale, roomGhostGlimpseScale, roomGhostGlimpseScale);
                roomGhost.SetActive(true);
                PlayGhostAppear();
            }
            yield return new WaitForSeconds(flickerInterval);

            flickerInterval *= lightFlickerSpeedMultiplier;
        }
        if (roomGhost != null) roomGhost.SetActive(false);

        // TV turns on with the ghost on screen - the room light flickers in
        // sync with it (dark whenever the TV is off) for a few cycles.
        if (tvGhost != null)
        {
            dialogue.ShowLine(tvGhostLine1, 3f);
            PlayTvNoise();
            for (int i = 0; i < tvFlickerCount; i++)
            {
                tvGhost.SetActive(true);
                PlayGhostAppear();
                blackOverlayGroup.alpha = 0f;
                PlayLightSwitchSound();
                yield return new WaitForSeconds(tvFlickerOnTime);

                tvGhost.SetActive(false);
                blackOverlayGroup.alpha = 1f;
                PlayLightSwitchSound();
                yield return new WaitForSeconds(tvFlickerOffTime);
            }
            tvGhost.SetActive(false);
            blackOverlayGroup.alpha = 0f;
            PlayLightSwitchSound();
            StopTvNoise();
        }

        // The ghost steps out of the TV and into the room through a fixed
        // set of waypoints - each one an instant pop, not a gradual grow-in.
        if (roomGhost != null)
        {
            for (int i = 0; i < approachPositions.Length; i++)
            {
                roomGhost.transform.position = approachPositions[i];
                float scale = i < approachScales.Length ? approachScales[i] : approachScales[approachScales.Length - 1];
                roomGhost.transform.localScale = new Vector3(scale, scale, scale);

                roomGhost.SetActive(true);
                PlayGhostAppear();
                yield return new WaitForSeconds(roomGhostOnTime);

                roomGhost.SetActive(false);
                yield return new WaitForSeconds(roomGhostOffTime);
            }

            dialogue.ShowLine(approachLine, 2f);
            yield return new WaitForSeconds(1.5f);
        }

        // A moment of relief - the light's on, the girl's nowhere to be seen...
        if (roomGhost != null) roomGhost.SetActive(false);
        PlayReliefSound();
        float reliefRevealTime = reliefLine.Length / Mathf.Max(dialogue.charactersPerSecond, 1f);
        dialogue.ShowLine(reliefLine, Mathf.Max(0f, reliefHoldTime - reliefRevealTime));
        yield return new WaitForSeconds(reliefHoldTime);

        StopReliefSound();

        // ...before she suddenly appears right in front of the player.
        if (roomGhost != null)
        {
            roomGhost.transform.position = jumpscarePosition;
            roomGhost.transform.localScale = new Vector3(jumpscareScale, jumpscareScale, jumpscareScale);
            roomGhost.SetActive(true);
        }
        PlayJumpscareSound();
        float jumpscareRevealTime = jumpscareLine.Length / Mathf.Max(dialogue.charactersPerSecond, 1f);
        dialogue.ShowLine(jumpscareLine, Mathf.Max(0f, jumpscareHoldTime - jumpscareRevealTime));
        yield return new WaitForSeconds(jumpscareHoldTime);

        // Cut to the ending.
        blackFade.FadeOut();
        yield return new WaitForSeconds(blackFade.fadeDuration);
        SceneManager.LoadScene(endingScene);
    }

    IEnumerator WaitForEPress()
    {
        while (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
        {
            yield return null;
        }
    }

    void PlayLightSwitchSound()
    {
        if (lightSwitchAudio != null && lightSwitchClip != null) lightSwitchAudio.PlayOneShot(lightSwitchClip);
    }

    void PlayGhostAppear()
    {
        if (ghostAppearAudio != null && ghostAppearClip != null) ghostAppearAudio.PlayOneShot(ghostAppearClip);
    }

    void PlayTvNoise()
    {
        if (tvAudio == null || tvNoiseClip == null) return;
        tvAudio.clip = tvNoiseClip;
        tvAudio.loop = true;
        tvAudio.Play();
    }

    void StopTvNoise()
    {
        if (tvAudio != null) tvAudio.Stop();
    }

    void PlayReliefSound()
    {
        if (breathingAudio != null && breathingClip != null)
        {
            breathingAudio.clip = breathingClip;
            breathingAudio.loop = true;
            breathingAudio.Play();
        }
        if (heartbeatAudio != null && heartbeatClip != null)
        {
            heartbeatAudio.clip = heartbeatClip;
            heartbeatAudio.loop = true;
            heartbeatAudio.Play();
        }
    }

    void StopReliefSound()
    {
        if (breathingAudio != null) breathingAudio.Stop();
        if (heartbeatAudio != null) heartbeatAudio.Stop();
    }

    void PlayJumpscareSound()
    {
        if (jumpscareAudio != null && jumpscareClip != null) jumpscareAudio.PlayOneShot(jumpscareClip);
        if (screamAudio != null && screamClip != null) screamAudio.PlayOneShot(screamClip);
    }
}
