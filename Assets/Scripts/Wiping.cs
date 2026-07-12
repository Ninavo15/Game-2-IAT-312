using System.Collections;
using UnityEngine;

// Runs the wiping mini-game loop: hold + drag over the blood stain to wipe
// it within the time limit. Win or lose, the blood is never actually gone -
// it snaps back and stress goes up either way.
public class Wipping : MonoBehaviour
{
    [Header("References")]
    public BloodWipeStain stain;
    public WipeCursor cursor;
    public Camera cam;
    public VignetteEffect stressVignette; // optional, plays a pulse each round

    [Header("UI (optional)")]
    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Text feedbackText;

    [Header("Round Settings")]
    public float roundDuration = 5f;
    [Range(0f, 1f)] public float successThreshold = 0.85f;
    public float resultHoldTime = 0.6f;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        StartCoroutine(RunRounds());
    }

    IEnumerator RunRounds()
    {
        while (true)
        {
            yield return StartCoroutine(RunSingleRound());
        }
    }

    IEnumerator RunSingleRound()
    {
        stain.ResetStain();
        SetFeedback("");

        float timeRemaining = roundDuration;
        while (timeRemaining > 0f)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 worldPos = cursor != null ? cursor.GetWorldPosition() : cam.ScreenToWorldPoint(Input.mousePosition);
                stain.WipeAtWorldPoint(worldPos);
            }

            timeRemaining -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(Mathf.Max(0f, timeRemaining)).ToString();
            yield return null;
        }

        bool success = stain.GetWipedFraction() >= successThreshold;
        SetFeedback(success ? "..." : "Still stained.");

        yield return new WaitForSeconds(resultHoldTime);

        // Regardless of outcome, the blood was never really removed.
        StressSystem.AddPoint(1);
        stressVignette?.TriggerVignette();
    }

    void SetFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
    }
}
