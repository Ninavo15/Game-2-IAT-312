using System.Collections;
using UnityEngine;

// Click the "Click to Rip" button repeatedly (no holding) to rip the poster
// down while the worker is turned left (safe). The character stays put - no
// walking around. If you click while the worker is turned right, you lose a
// heart; losing all hearts (or running out of the time limit) gets you
// caught. Ripping removes visible pieces of the poster directly, so progress
// is shown by the poster itself tearing apart.
public class PosterRipMiniGame : MonoBehaviour
{
    [Header("References")]
    public WorkerAI worker;
    public PosterTear poster;
    public ExclamationCountdown exclamationMark; // shown above the worker when caught

    [Header("Character Hand Swap")]
    public SpriteRenderer characterSprite;
    public Sprite characterNormalSprite;
    public Sprite characterHandUpSprite;
    public float handUpFlashDuration = 0.15f;

    [Header("UI (optional)")]
    public GameObject ripButton; // "Click to Rip" button, shown only while playing
    public UnityEngine.UI.Text timerText;
    public float timerWarningThreshold = 5f;
    public Color timerWarningColor = Color.red;
    public UnityEngine.UI.Image[] heartIcons;

    [Header("Start Gate")]
    public GameObject startOverlay; // shown until the player clicks the start button

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
    public float ripPerClick = 0.05f; // 20 clicks to fully rip
    [Range(0f, 1f)] public float successThreshold = 1f;
    public float timeLimit = 20f; // seconds, once ripping starts - placeholder for now
    public int maxHearts = 3;

    [Header("Dialogue + Transition")]
    public SceneTransitionAfterDialogue winTransition;
    public string winLine = "Got it! Time to get out of here.";
    public SceneTransitionAfterDialogue caughtTransition; // all hearts lost
    public string caughtLine = "Oh no, they saw me!";
    public SceneTransitionAfterDialogue timeUpTransition; // time ran out
    public string timeUpLine = "Shucks, the employer is noticing, I need to stop";

    bool gameStarted;
    bool startClicked;
    float ripProgress;
    bool finished;
    float timeRemaining;
    int heartsRemaining;
    bool heartLostThisCheck;
    bool wasChecking;
    CharacterMovement characterMovement;
    Color timerDefaultColor;
    Coroutine handFlashRoutine;

    void Start()
    {
        if (characterSprite != null) characterMovement = characterSprite.GetComponent<CharacterMovement>();
        if (exclamationMark != null) exclamationMark.Hide();
        if (countdownOverlay != null) countdownOverlay.SetActive(false);
        if (resultOverlay != null) resultOverlay.SetActive(false);
        if (timerText != null) timerDefaultColor = timerText.color;
        timeRemaining = timeLimit;
        heartsRemaining = maxHearts;
        SetCharacterHandUp(false);

        // The character never moves during this mini-game.
        if (characterMovement != null) characterMovement.enabled = false;

        // Hidden until the countdown finishes - only the start overlay shows at first.
        if (timerText != null) timerText.enabled = false;
        SetHeartIconsVisible(false);
        if (ripButton != null) ripButton.SetActive(false);

        StartCoroutine(StartSequence());
    }

    // Wired to the start button's OnClick.
    public void OnStartButtonClicked()
    {
        startClicked = true;
    }

    IEnumerator StartSequence()
    {
        startClicked = false;
        if (startOverlay != null) startOverlay.SetActive(true);
        while (!startClicked)
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
        if (ripButton != null) ripButton.SetActive(true);
        if (worker != null) worker.StartBehavior();

        gameStarted = true;
    }

    void Update()
    {
        if (!gameStarted || finished) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerText();
        if (timeRemaining <= 0f)
        {
            TriggerTimeUp();
            return;
        }

        // Only reset right when the worker turns back to Safe (edge-triggered) -
        // calling Hide() every frame while not checking would cancel WorkerAI's
        // own warning countdown, which also runs while IsChecking is still false.
        bool checking = worker != null && worker.IsChecking;
        if (checking)
        {
            wasChecking = true;
        }
        else if (wasChecking)
        {
            wasChecking = false;
            heartLostThisCheck = false;
            if (exclamationMark != null) exclamationMark.Hide();
        }
    }

    // Wired to the rip button's OnClick - one click, one small tear.
    public void OnRipButtonClicked()
    {
        if (!gameStarted || finished) return;

        bool checking = worker != null && worker.IsChecking;
        if (checking)
        {
            // The worker only notices if the player is actually caught mid-rip.
            if (exclamationMark != null) exclamationMark.ShowDanger();
            if (!heartLostThisCheck)
            {
                heartLostThisCheck = true;
                LoseHeart();
            }
            return;
        }

        FlashHandUp();

        ripProgress = Mathf.Clamp01(ripProgress + ripPerClick);
        if (poster != null) poster.SetProgress(ripProgress);

        if (ripProgress >= successThreshold)
        {
            TriggerWin();
        }
    }

    void FlashHandUp()
    {
        if (handFlashRoutine != null) StopCoroutine(handFlashRoutine);
        handFlashRoutine = StartCoroutine(FlashHandUpRoutine());
    }

    IEnumerator FlashHandUpRoutine()
    {
        SetCharacterHandUp(true);
        yield return new WaitForSeconds(handUpFlashDuration);
        SetCharacterHandUp(false);
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
        timerText.color = timeRemaining <= timerWarningThreshold ? timerWarningColor : timerDefaultColor;
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
        SetCharacterHandUp(false);
        if (ripButton != null) ripButton.SetActive(false);
        if (worker != null) worker.StopBehavior();

        StartCoroutine(ShowResultThenProceed("WIN", "The poster is being removed", () =>
        {
            PosterState.Removed = true;
            PosterState.Attempted = true;
            if (winTransition != null) winTransition.ShowThenLoad(winLine);
        }));
    }

    // Time ran out but the player still had at least one heart left - the
    // poster stays up in Scene 3.
    void TriggerTimeUp()
    {
        finished = true;
        SetCharacterHandUp(false);
        if (ripButton != null) ripButton.SetActive(false);
        if (worker != null) worker.StopBehavior();
        StressSystem.AddPoint(1);

        StartCoroutine(ShowResultThenProceed("LOSE", "the poster still there, your stress level up", () =>
        {
            PosterState.Attempted = true;
            if (timeUpTransition != null) timeUpTransition.ShowThenLoad(timeUpLine);
        }));
    }

    // All hearts lost - the worker fully catches the player.
    void TriggerCaughtByWorker()
    {
        finished = true;
        SetCharacterHandUp(false);
        if (ripButton != null) ripButton.SetActive(false);
        if (worker != null) worker.StopBehavior();
        if (exclamationMark != null) exclamationMark.ShowDanger();
        StressSystem.AddPoint(1);

        StartCoroutine(ShowResultThenProceed("LOSE", "You being caught by the worker", () =>
        {
            PosterState.Attempted = true;
            if (caughtTransition != null) caughtTransition.ShowThenLoad(caughtLine);
        }));
    }
}
