using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Wipping : MonoBehaviour
{
    [Header("References")]
    public WipeFade stain;
    public SpriteRenderer stainRenderer; // bounds used to check the cursor is over the car
    public WipeCursor cursor;
    public Camera cam;

    [Header("Scene Transition")]
    public Fading fade;
    public string sceneToLoad = "Scene 3";

    [Header("UI (optional)")]
    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Image progressBarFill;
    public GameObject resultOverlay; // Lose Win screen
    public UnityEngine.UI.Text resultTitleText; // "Win" / "Lose"
    public UnityEngine.UI.Text resultSubtitleText; // detail message

    [Header("Intro Overlay")]
    [FormerlySerializedAs("countdownOverlay")]
    public GameObject introOverlay; // shown until the player starts wiping the car
    public GameObject gameOverlay; // timer + bar + instructions, shown once playing starts
    public GameObject handCursorObject;

    [Header("Round Settings")]
    public float roundDuration = 15f;
    [Range(0.01f, 1f)] public float successThreshold = 0.75f;
    public float resultHoldTime = 2.5f;
    public float timerWarningThreshold = 5f;
    public Color timerWarningColor = Color.red;

    [Header("Wipe Speed")]
    // Holding the mouse
    public float baseWipeRate = 0.35f;
    // Extra wipe rate per second added on top, scaled by how fast the mouse dragging.
    public float wipeRatePerSpeedUnit = 0.15f;

    [Header("Wipe Sound")]
    public AudioSource wipeAudio;
    public AudioClip wipeClip;

    Vector3 lastCursorWorldPos;
    float progressBarFullWidth;
    Color timerDefaultColor;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        if (resultOverlay != null) resultOverlay.SetActive(false);
        if (progressBarFill != null) progressBarFullWidth = progressBarFill.rectTransform.sizeDelta.x;
        if (timerText != null) timerDefaultColor = timerText.color;

        if (wipeAudio != null)
        {
            wipeAudio.clip = wipeClip;
            wipeAudio.loop = true;
        }

        SetPlayingVisible(false);
        StartCoroutine(RunGame());
    }

    IEnumerator RunGame()
    {
        yield return StartCoroutine(WaitForFirstWipe());
        yield return StartCoroutine(RunSingleRound());
    }

    // The intro overlay stays up until the player actually starts dragging
    // over the car - that same input is the round's first wipe.
    IEnumerator WaitForFirstWipe()
    {
        if (introOverlay != null) introOverlay.SetActive(true);
        SetPlayingVisible(false);

        while (!(Input.GetMouseButton(0) && IsOverCar(GetCursorWorldPos())))
        {
            yield return null;
        }

        if (introOverlay != null) introOverlay.SetActive(false);
        SetPlayingVisible(true);
    }

    void SetPlayingVisible(bool visible)
    {
        if (gameOverlay != null) gameOverlay.SetActive(visible);
        if (handCursorObject != null) handCursorObject.SetActive(visible);
    }

    IEnumerator RunSingleRound()
    {
        stain.ResetStain();
        lastCursorWorldPos = GetCursorWorldPos();

        float timeRemaining = roundDuration;
        bool success = false;
        while (timeRemaining > 0f)
        {
            Vector3 worldPos = GetCursorWorldPos();

            bool wiping = Input.GetMouseButton(0) && IsOverCar(worldPos);
            if (wiping)
            {
                float speed = Vector3.Distance(worldPos, lastCursorWorldPos) / Mathf.Max(Time.deltaTime, 0.0001f);
                stain.WipeAtWorldPoint(worldPos, (baseWipeRate + speed * wipeRatePerSpeedUnit) * Time.deltaTime);
            }
            if (wipeAudio != null)
            {
                if (wiping && !wipeAudio.isPlaying) wipeAudio.Play();
                else if (!wiping && wipeAudio.isPlaying) wipeAudio.Stop();
            }
            lastCursorWorldPos = worldPos;

            timeRemaining -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(Mathf.Max(0f, timeRemaining)).ToString();
                timerText.color = timeRemaining <= timerWarningThreshold ? timerWarningColor : timerDefaultColor;
            }
            if (progressBarFill != null)
            {
                float displayWiped = Mathf.Min(stain.GetWipedFraction() / successThreshold, 1f);
                Vector2 size = progressBarFill.rectTransform.sizeDelta;
                size.x = progressBarFullWidth * (1f - displayWiped);
                progressBarFill.rectTransform.sizeDelta = size;
            }

            if (stain.GetWipedFraction() >= successThreshold)
            {
                success = true;
                stain.CompleteWipe(); // snap to fully clean
                break;
            }

            yield return null;
        }

        if (wipeAudio != null) wipeAudio.Stop();
        ShowResult(success);

        yield return new WaitForSeconds(resultHoldTime);

        if (resultOverlay != null) resultOverlay.SetActive(false);

        StressSystem.AddPoint(1);

        CarWashState.Cleaned = success;
        CarWashState.Played = true;

        if (fade != null)
        {
            fade.FadeOut();
            yield return new WaitForSeconds(fade.fadeDuration);
        }

        if (!string.IsNullOrEmpty(sceneToLoad)) SceneManager.LoadScene(sceneToLoad);
    }

    void ShowResult(bool success)
    {
        if (resultOverlay != null) resultOverlay.SetActive(true);
        if (resultTitleText != null)
        {
            resultTitleText.text = success ? "Win" : "Lose";
        }
        if (resultSubtitleText != null)
        {
            resultSubtitleText.text = success
                ? "Your car is cleaned."
                : "Your car is NOT cleaned. Your stress increases.";
        }
    }

    Vector3 GetCursorWorldPos()
    {
        return cursor != null ? cursor.GetWorldPosition() : cam.ScreenToWorldPoint(Input.mousePosition);
    }

    bool IsOverCar(Vector3 worldPos)
    {
        Bounds b = stainRenderer.bounds;
        return worldPos.x >= b.min.x && worldPos.x <= b.max.x && worldPos.y >= b.min.y && worldPos.y <= b.max.y;
    }
}
