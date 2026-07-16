using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Hold the mouse button near the poster to rip it down while the worker is
// turned left (safe)
// you lose a heart if worker is checking - 3 lose
// Rip in time limit to win, or lose if time runs out
[RequireComponent(typeof(Collider2D))]
public class PosterRipMiniGame : MonoBehaviour
{
    [Header("References")]
    public WorkerAI worker;
    public PosterTear poster;
    public ExclamationMarkAnimator exclamationMark; // shown above the worker when caught

    [Header("Character Hand Swap")]
    public SpriteRenderer characterSprite;
    public Sprite characterNormalSprite;
    public Sprite characterHandUpSprite;

    [Header("UI (optional)")]
    public GameObject promptText; // "Click to rip poster"
    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Image[] heartIcons;

    [Header("Start Gate")]
    public GameObject startOverlay; // shown until the player presses E

    [Header("Countdown")]
    public GameObject countdownOverlay;
    public UnityEngine.UI.Text countdownText;
    public float countdownStepDuration = 1f;

    [Header("Result Overlay")]
    public GameObject resultOverlay;
    public UnityEngine.UI.Text resultTitleText;
    public UnityEngine.UI.Text resultSubtitleText;
    public float resultHoldTime = 2.5f;

    [Header("Rip Settings")]
    public float ripRatePerSecond = 0.2f;
    [Range(0f, 1f)] public float successThreshold = 1f;
    public float timeLimit = 30f; // seconds, once ripping starts - placeholder for now
    public int maxHearts = 3;

    [Header("Dialogue + Transition")]
    public SceneTransitionAfterDialogue winTransition;
    public string winLine = "Got it! Time to get out of here.";
    public SceneTransitionAfterDialogue caughtTransition; // all hearts lost
    public string caughtLine = "Oh no, they saw me!";
    public SceneTransitionAfterDialogue timeUpTransition; // time ran out, hearts still left
    public string timeUpLine = "I'm out of time...";

    bool playerInRange;
    bool gameStarted;
    float ripProgress;
    bool finished;
    float timeRemaining;
    int heartsRemaining;
    bool heartLostThisCheck;
    CharacterMovement characterMovement;

    void Start()
    {
        if (characterSprite != null) characterMovement = characterSprite.GetComponent<CharacterMovement>();
        if (exclamationMark != null) exclamationMark.Hide();
        if (promptText != null) promptText.SetActive(false);
        if (countdownOverlay != null) countdownOverlay.SetActive(false);
        if (resultOverlay != null) resultOverlay.SetActive(false);
        timeRemaining = timeLimit;
        heartsRemaining = maxHearts;
        SetCharacterHandUp(false);

        // Hidden until the countdown finishes - only the start overlay shows at first.
        if (timerText != null) timerText.enabled = false;
        SetHeartIconsVisible(false);
        if (characterMovement != null) characterMovement.enabled = false;

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        if (startOverlay != null) startOverlay.SetActive(true);
        while (!(Keyboard.current != null && Keyboard.current[Key.E].wasPressedThisFrame))
        {
            yield return null;
        }
        if (startOverlay != null) startOverlay.SetActive(false);

        if (countdownOverlay != null) countdownOverlay.SetActive(true);
        string[] steps = { "3", "2", "1", "START" };
        foreach (string step in steps)
        {
            if (countdownText != null) countdownText.text = step;
            yield return new WaitForSeconds(countdownStepDuration);
        }
        if (countdownOverlay != null) countdownOverlay.SetActive(false);

        if (timerText != null) timerText.enabled = true;
        UpdateTimerText();
        SetHeartIconsVisible(true);
        UpdateHeartsUI();
        if (characterMovement != null) characterMovement.enabled = true;
        if (worker != null) worker.StartBehavior();

        gameStarted = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = false;
    }

    void Update()
    {
        if (!gameStarted || finished) return;

        bool holding = playerInRange && Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (promptText != null) promptText.SetActive(playerInRange && !holding);
        SetCharacterHandUp(holding);

        timeRemaining -= Time.deltaTime;
        UpdateTimerText();
        if (timeRemaining <= 0f)
        {
            TriggerTimeUp();
            return;
        }

        bool checking = worker != null && worker.IsChecking;

        if (checking)
        {
            if (holding && !heartLostThisCheck)
            {
                heartLostThisCheck = true;
                LoseHeart();
            }
            return;
        }
        heartLostThisCheck = false;

        if (!holding) return;

        ripProgress = Mathf.Clamp01(ripProgress + ripRatePerSecond * Time.deltaTime);
        if (poster != null) poster.SetProgress(ripProgress);

        if (ripProgress >= successThreshold)
        {
            TriggerWin();
        }
    }

    void SetCharacterHandUp(bool handUp)
    {
        if (characterSprite == null) return;

        bool facingRight = characterMovement != null ? characterMovement.FacingRight : !characterSprite.flipX;

        if (handUp)
        {
            characterSprite.sprite = characterHandUpSprite;
            characterSprite.flipX = !facingRight;
        }
        else
        {
            characterSprite.sprite = characterNormalSprite;
            characterSprite.flipX = !facingRight;
        }
    }

    void UpdateTimerText()
    {
        if (timerText == null) return;
        timerText.text = Mathf.CeilToInt(Mathf.Max(0f, timeRemaining)).ToString();
    }

    void UpdateHeartsUI()
    {
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null) heartIcons[i].enabled = i < heartsRemaining;
        }
    }

    void SetHeartIconsVisible(bool visible)
    {
        if (heartIcons == null) return;
        foreach (var heart in heartIcons)
        {
            if (heart != null) heart.enabled = visible;
        }
    }

    void LoseHeart()
    {
        heartsRemaining = Mathf.Max(0, heartsRemaining - 1);
        UpdateHeartsUI();
        if (heartsRemaining <= 0)
        {
            TriggerCaughtByWorker();
        }
    }

    IEnumerator ShowResultThenProceed(string title, string subtitle, System.Action after)
    {
        if (resultOverlay != null) resultOverlay.SetActive(true);
        if (resultTitleText != null) resultTitleText.text = title;
        if (resultSubtitleText != null) resultSubtitleText.text = subtitle;

        yield return new WaitForSeconds(resultHoldTime);

        if (resultOverlay != null) resultOverlay.SetActive(false);
        after?.Invoke();
    }

    void TriggerWin()
    {
        finished = true;
        if (promptText != null) promptText.SetActive(false);
        SetCharacterHandUp(false);
        if (worker != null) worker.StopBehavior();

        StartCoroutine(ShowResultThenProceed("WIN", "The poster is being removed", () =>
        {
            PosterState.Removed = true;
            if (winTransition != null) winTransition.ShowThenLoad(winLine);
        }));
    }

    // Time ran out but the player still had at least one heart left - the
    // poster stays up in Scene 3.
    void TriggerTimeUp()
    {
        finished = true;
        if (promptText != null) promptText.SetActive(false);
        SetCharacterHandUp(false);
        if (worker != null) worker.StopBehavior();
        StressSystem.AddPoint(1);

        StartCoroutine(ShowResultThenProceed("LOSE", "the poster still there, your stress level up", () =>
        {
            if (timeUpTransition != null) timeUpTransition.ShowThenLoad(timeUpLine);
        }));
    }

    // All hearts lost - the worker fully catches the player.
    void TriggerCaughtByWorker()
    {
        finished = true;
        if (promptText != null) promptText.SetActive(false);
        SetCharacterHandUp(false);
        if (worker != null) worker.StopBehavior();
        if (exclamationMark != null) exclamationMark.Show();
        StressSystem.AddPoint(1);

        StartCoroutine(ShowResultThenProceed("LOSE", "You being caught by the worker", () =>
        {
            if (caughtTransition != null) caughtTransition.ShowThenLoad(caughtLine);
        }));
    }
}
