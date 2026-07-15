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

    [Header("Rip Settings")]
    public float ripRatePerSecond = 0.2f;
    [Range(0f, 1f)] public float successThreshold = 1f;
    public float timeLimit = 30f; // seconds, once ripping starts - placeholder for now
    public int maxHearts = 3;

    [Header("Dialogue + Transition")]
    public SceneTransitionAfterDialogue winTransition;
    public string winLine = "Got it! Time to get out of here.";
    public SceneTransitionAfterDialogue caughtTransition;
    public string caughtLine = "Oh no, they saw me!";
    public string timeUpLine = "I'm out of time...";

    bool playerInRange;
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
        timeRemaining = timeLimit;
        heartsRemaining = maxHearts;
        UpdateTimerText();
        UpdateHeartsUI();
        SetCharacterHandUp(false);
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
        if (finished) return;

        bool holding = playerInRange && Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (promptText != null) promptText.SetActive(playerInRange && !holding);
        SetCharacterHandUp(holding);

        timeRemaining -= Time.deltaTime;
        UpdateTimerText();
        if (timeRemaining <= 0f)
        {
            TriggerCaught(timeUpLine);
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

    void LoseHeart()
    {
        heartsRemaining = Mathf.Max(0, heartsRemaining - 1);
        UpdateHeartsUI();
        if (heartsRemaining <= 0)
        {
            TriggerCaught(caughtLine);
        }
    }

    void TriggerCaught(string line)
    {
        finished = true;
        if (promptText != null) promptText.SetActive(false);
        SetCharacterHandUp(false);
        if (worker != null) worker.StopBehavior();
        if (exclamationMark != null) exclamationMark.Show();
        StressSystem.AddPoint(1);
        if (caughtTransition != null) caughtTransition.ShowThenLoad(line);
    }

    void TriggerWin()
    {
        finished = true;
        if (promptText != null) promptText.SetActive(false);
        SetCharacterHandUp(false);
        if (worker != null) worker.StopBehavior();
        if (winTransition != null) winTransition.ShowThenLoad(winLine);
    }
}
