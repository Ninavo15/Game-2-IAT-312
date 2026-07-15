using System.Collections;
using UnityEngine;

// Alternates the worker between "Safe" (looking away, busy) and "Checking"
// (turned toward the player) on a randomized timer. Flips the sprite so the
// player has a visual cue for which state it's in.
public class WorkerAI : MonoBehaviour
{
    [Header("Safe (looking away)")]
    public float minSafeDuration = 2f;
    public float maxSafeDuration = 5f;

    [Header("Checking (turned toward player)")]
    public float minCheckDuration = 1.5f;
    public float maxCheckDuration = 3f;

    [Header("Position per facing")]
    public float leftPositionX = -14.78f;
    public float rightPositionX = 7.92f;
    public float scaleX = 16.04629f;

    [Header("Warning")]
    public ExclamationMarkAnimator exclamationMark;
    public float warningLeadTime = 2f;

    public bool IsChecking { get; private set; }

    Vector3 baseScale;
    Vector3 basePosition;

    void Awake()
    {
        baseScale = transform.localScale;
        basePosition = transform.localPosition;
    }

    void Start()
    {
        StartCoroutine(AttentionLoop());
    }

    IEnumerator AttentionLoop()
    {
        while (true)
        {
            // Safe: turned left (mirrored from the default right-facing art).
            IsChecking = false;
            SetFacingLeft(true);
            if (exclamationMark != null) exclamationMark.Hide();

            float safeDuration = Random.Range(minSafeDuration, maxSafeDuration);
            float warnLead = Mathf.Min(warningLeadTime, safeDuration);
            yield return new WaitForSeconds(safeDuration - warnLead);

            // Warn the player shortly before the worker turns to check.
            if (exclamationMark != null) exclamationMark.Show();
            yield return new WaitForSeconds(warnLead);

            // Checking: turned right (default art orientation).
            IsChecking = true;
            SetFacingLeft(false);
            if (exclamationMark != null) exclamationMark.Hide();
            yield return new WaitForSeconds(Random.Range(minCheckDuration, maxCheckDuration));
        }
    }

    // Called once the mini-game ends so the loop doesn't keep moving/flipping
    // the worker or toggling the warning icon during the win/caught transition.
    public void StopBehavior()
    {
        StopAllCoroutines();
    }

    // Flips via the transform's own scale rather than SpriteRenderer.flipX so the
    // turn mirrors in place regardless of whether the sprite's pivot is centered,
    // and slides to the position matching that facing direction.
    void SetFacingLeft(bool faceLeft)
    {
        float sign = faceLeft ? -1f : 1f;
        transform.localScale = new Vector3(Mathf.Abs(scaleX) * sign, baseScale.y, baseScale.z);

        Vector3 pos = basePosition;
        pos.x = faceLeft ? leftPositionX : rightPositionX;
        transform.localPosition = pos;
    }
}
