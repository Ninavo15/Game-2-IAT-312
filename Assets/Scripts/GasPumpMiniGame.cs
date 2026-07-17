using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Hold E to push the indicator up a vertical gauge; releasing lets gravity
// pull it back down. Fuel only fills while the indicator is inside the green
// zone (and the key is held), within a 30-second countdown. The green zone
// jumps to a new random spot on the track every zoneMoveInterval seconds, so
// the player has to keep re-tracking it. Filling the tank before time runs
// out wins the round and records a best time (PlayerPrefs); running out of
// time loses it. Either way, the amount of fuel pumped is saved to
// GasPumpState and the game fades out and loads nextSceneName ("Scene 3"),
// matching how the other mini-games (e.g. Ripping Poster) hand control back.
public class GasPumpMiniGame : MonoBehaviour
{
    [Header("Overlays")]
    [FormerlySerializedAs("countdownOverlay")]
    public GameObject introOverlay; // shown until the player starts pumping
    public GameObject resultOverlay;
    public UnityEngine.UI.Text resultTitleText;
    public UnityEngine.UI.Text resultSubtitleText;
    public GameObject hud; // "Game" HUD parent - gauge, fuel bar, timer, etc.

    [Header("Gauge")]
    public RectTransform gaugeTrack; // its rect.height is the indicator's travel range
    public RectTransform indicator;  // child of gaugeTrack, anchoredPosition.y = 0..trackHeight
    public RectTransform greenZone;  // child of gaugeTrack; anchoredPosition.y +/- half sizeDelta.y = zone bounds
    public float riseAcceleration = 2200f;
    public float gravity = 1300f;
    public float maxSpeed = 900f;
    public float zoneHeight = 70f; // green zone height, enforced on greenZone every round
    public float zoneMoveInterval = 5f; // seconds between the green zone jumping to a new spot

    [Header("Fuel")]
    public UnityEngine.UI.Image fuelFill; // Image.Type = Filled, Horizontal
    public UnityEngine.UI.Text fuelText;
    public float fuelTarget = 100f;
    public float fuelFillRate = 10f; // liters/second while holding and in the green zone

    [Header("Timer / Best")]
    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Text bestText;
    public float timeLimit = 30f;
    const string BestTimePrefKey = "GasPump_BestTime";

    [Header("Input")]
    public KeyCode pumpKey = KeyCode.E;
    public UnityEngine.UI.Text holdPromptText;
    public Color holdPromptIdleColor = new Color(1, 1, 1, 0.55f);
    public Color holdPromptActiveColor = new Color(1, 0.85f, 0.2f, 1f);

    [Header("Result")]
    public float resultHoldTime = 2.5f;

    [Header("Borders")]
    public float borderThickness = 4f;
    public Color borderColor = Color.white;

    [Header("Scene Transition")]
    public string nextSceneName = "Scene 3";
    public float transitionFadeDuration = 1.5f;

    [Header("Car Appearance")]
    public string carObjectName = "car bloody side";
    public Sprite carCleanSprite; // assign the "car side no blood" sprite in the Inspector

    bool gameActive;
    bool finished;
    float indicatorPos;
    float indicatorVelocity;
    float fuel;
    float elapsedTime;
    float timeRemaining;
    float bestTime;
    float zoneMoveTimer;
    UnityEngine.UI.Image fadeOverlay;

    void Start()
    {
        bestTime = PlayerPrefs.GetFloat(BestTimePrefKey, 0f);
        UpdateBestText();
        SetHoldPromptActive(false);
        if (holdPromptText != null) holdPromptText.text = $"HOLD [{pumpKey}] TO PUMP";
        UpdateFuelUI();
        ApplyCarCleanliness();

        AddBorder(gaugeTrack, borderThickness, borderColor);
        if (fuelFill != null) AddBorder(fuelFill.rectTransform.parent as RectTransform, borderThickness, borderColor);

        fadeOverlay = CreateFadeOverlay();

        ShowIntroScreen();
    }

    void ShowIntroScreen()
    {
        finished = false;
        gameActive = false;
        if (hud != null) hud.SetActive(false);
        if (resultOverlay != null) resultOverlay.SetActive(false);
        if (introOverlay != null) introOverlay.SetActive(true);
        StartCoroutine(WaitForFirstPump());
    }

    // The intro overlay stays up until the player actually holds the pump
    // key - that same key-hold is the round's first pump.
    IEnumerator WaitForFirstPump()
    {
        while (!Input.GetKey(pumpKey))
        {
            yield return null;
        }
        if (introOverlay != null) introOverlay.SetActive(false);

        ResetRound();
        if (hud != null) hud.SetActive(true);
        gameActive = true;
    }

    void ResetRound()
    {
        fuel = 0f;
        elapsedTime = 0f;
        timeRemaining = timeLimit;
        indicatorPos = 0f;
        indicatorVelocity = 0f;
        zoneMoveTimer = zoneMoveInterval;
        RandomizeZonePosition();
        UpdateIndicatorVisual();
        UpdateFuelUI();
        UpdateTimerText();
    }

    void Update()
    {
        if (!gameActive || finished) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateTimerText();
            TriggerTimeUp();
            return;
        }

        bool holding = Input.GetKey(pumpKey);
        SetHoldPromptActive(holding);

        indicatorVelocity += (holding ? riseAcceleration : 0f) * Time.deltaTime;
        indicatorVelocity -= gravity * Time.deltaTime;
        indicatorVelocity = Mathf.Clamp(indicatorVelocity, -maxSpeed, maxSpeed);

        float trackHeight = gaugeTrack != null ? gaugeTrack.rect.height : 0f;
        indicatorPos += indicatorVelocity * Time.deltaTime;
        if (indicatorPos <= 0f)
        {
            indicatorPos = 0f;
            indicatorVelocity = 0f;
        }
        else if (indicatorPos >= trackHeight)
        {
            indicatorPos = trackHeight;
            indicatorVelocity = 0f;
        }
        UpdateIndicatorVisual();

        zoneMoveTimer -= Time.deltaTime;
        if (zoneMoveTimer <= 0f)
        {
            zoneMoveTimer += zoneMoveInterval;
            RandomizeZonePosition();
        }

        if (holding && IsInGreenZone(indicatorPos))
        {
            fuel = Mathf.Min(fuelTarget, fuel + fuelFillRate * Time.deltaTime);
        }

        elapsedTime += Time.deltaTime;
        UpdateFuelUI();
        UpdateTimerText();

        if (fuel >= fuelTarget)
        {
            TriggerWin();
        }
    }

    // Mirrors CarBloodStain's swap (used in Scene 3) here, since the car in
    // this scene isn't wired to that component - if the player already
    // washed the car clean in the Wipe Mini Game, show it clean here too.
    void ApplyCarCleanliness()
    {
        if (!CarWashState.Cleaned || carCleanSprite == null) return;
        GameObject car = GameObject.Find(carObjectName);
        if (car == null) return;
        var sr = car.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = carCleanSprite;
    }

    // Drops a solid-color panel behind target, sized thickness bigger on every
    // side, so it peeks out as a frame. Grown around target's own pivot so it
    // works regardless of whether target uses a corner or center pivot.
    static void AddBorder(RectTransform target, float thickness, Color color)
    {
        if (target == null || target.parent == null) return;

        GameObject borderGO = new GameObject(target.name + " Border", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        borderGO.layer = target.gameObject.layer;
        borderGO.transform.SetParent(target.parent, false);

        RectTransform borderRect = borderGO.GetComponent<RectTransform>();
        borderRect.anchorMin = target.anchorMin;
        borderRect.anchorMax = target.anchorMax;
        borderRect.pivot = target.pivot;
        Vector2 pad = new Vector2(thickness, thickness);
        borderRect.sizeDelta = target.sizeDelta + pad * 2f;
        borderRect.anchoredPosition = target.anchoredPosition + pad * (2f * target.pivot - Vector2.one);

        var image = borderGO.GetComponent<UnityEngine.UI.Image>();
        image.color = color;
        image.raycastTarget = false;

        borderGO.transform.SetSiblingIndex(target.GetSiblingIndex());
    }

    // Full-screen, initially-transparent black panel on top of everything
    // else in the canvas (including the result overlay), used to fade to
    // black before loading the next scene.
    UnityEngine.UI.Image CreateFadeOverlay()
    {
        Transform canvasRoot = resultOverlay != null ? resultOverlay.transform.parent : null;
        if (canvasRoot == null) return null;

        GameObject go = new GameObject("Fade Overlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        go.layer = resultOverlay.layer;
        go.transform.SetParent(canvasRoot, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var image = go.GetComponent<UnityEngine.UI.Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;
        go.transform.SetAsLastSibling();
        return image;
    }

    IEnumerator FadeAndLoadScene()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.raycastTarget = true;
            Color c = fadeOverlay.color;
            float t = 0f;
            while (t < transitionFadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / transitionFadeDuration);
                fadeOverlay.color = c;
                yield return null;
            }
            c.a = 1f;
            fadeOverlay.color = c;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    void RandomizeZonePosition()
    {
        if (greenZone == null || gaugeTrack == null) return;
        greenZone.sizeDelta = new Vector2(greenZone.sizeDelta.x, zoneHeight);
        float trackHeight = gaugeTrack.rect.height;
        float half = zoneHeight * 0.5f;
        float newY = Random.Range(half, Mathf.Max(half, trackHeight - half));
        greenZone.anchoredPosition = new Vector2(greenZone.anchoredPosition.x, newY);
    }

    bool IsInGreenZone(float pos)
    {
        if (greenZone == null) return false;
        float half = greenZone.sizeDelta.y * 0.5f;
        float center = greenZone.anchoredPosition.y;
        return pos >= center - half && pos <= center + half;
    }

    void UpdateIndicatorVisual()
    {
        if (indicator == null) return;
        Vector2 pos = indicator.anchoredPosition;
        pos.y = indicatorPos;
        indicator.anchoredPosition = pos;
    }

    void UpdateFuelUI()
    {
        float progress = fuelTarget > 0f ? Mathf.Clamp01(fuel / fuelTarget) : 0f;

        // Drive the fill by width (anchorMax.x) instead of Image.fillAmount -
        // this only depends on anchors, so it can't be broken by the Image's
        // Type/FillMethod dropdown getting changed in the Editor.
        if (fuelFill != null)
        {
            RectTransform fillRect = fuelFill.rectTransform;
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(progress, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
        }

        if (fuelText != null) fuelText.text = $"FUEL IN TANK\n{Mathf.RoundToInt(fuel)} / {Mathf.RoundToInt(fuelTarget)} L";
    }

    void UpdateTimerText()
    {
        if (timerText == null) return;
        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }

    void UpdateBestText()
    {
        if (bestText == null) return;
        bestText.text = bestTime > 0f ? bestTime.ToString("0.0") : "--";
    }

    void SetHoldPromptActive(bool holding)
    {
        if (holdPromptText == null) return;
        holdPromptText.color = holding ? holdPromptActiveColor : holdPromptIdleColor;
    }

    void TriggerWin()
    {
        finished = true;
        gameActive = false;
        SetHoldPromptActive(false);

        bool newBest = bestTime <= 0f || elapsedTime < bestTime;
        if (newBest)
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetFloat(BestTimePrefKey, bestTime);
            PlayerPrefs.Save();
        }
        UpdateBestText();
        GasPumpState.FuelPumped = fuel;
        GasPumpState.Attempted = true;

        if (resultTitleText != null) resultTitleText.text = "FULL TANK!";
        if (resultSubtitleText != null)
        {
            resultSubtitleText.text = newBest
                ? $"New best time: {elapsedTime:0.0}s!"
                : $"Time: {elapsedTime:0.0}s (Best: {bestTime:0.0}s)";
        }
        if (resultOverlay != null) resultOverlay.SetActive(true);

        StartCoroutine(FinishRoundAfterDelay());
    }

    // Timer ran out before the tank was full.
    void TriggerTimeUp()
    {
        finished = true;
        gameActive = false;
        SetHoldPromptActive(false);
        GasPumpState.FuelPumped = fuel;
        GasPumpState.Attempted = true;

        if (resultTitleText != null) resultTitleText.text = "OUT OF TIME!";
        if (resultSubtitleText != null)
        {
            resultSubtitleText.text = $"Only pumped {Mathf.RoundToInt(fuel)} / {Mathf.RoundToInt(fuelTarget)} L";
        }
        if (resultOverlay != null) resultOverlay.SetActive(true);

        StartCoroutine(FinishRoundAfterDelay());
    }

    IEnumerator FinishRoundAfterDelay()
    {
        yield return new WaitForSeconds(resultHoldTime);
        StartCoroutine(FadeAndLoadScene());
    }
}
