using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button howToPlayButton;
    public CanvasGroup menuButtonsGroup;

    [Header("How To Play")]
    public CanvasGroup howToPlayGroup;

    [Header("Background / Car")]
    public ScrollingBackground background;
    public AudioSource carEngineAudio;
    public float speedUpMultiplier = 3f;
    public float speedUpDuration = 2f;
    public float holdBeforeFadeDuration = 1f;

    [Header("Fade / Scene")]
    public Fading fade;
    public string nextScene = "Prolouge";

    public float uiFadeDuration = 0.4f;

    public void OnStartPressed()
    {
        startButton.interactable = false;
        howToPlayButton.interactable = false;
        StartCoroutine(StartSequence());
    }

    public void OnHowToPlayPressed()
    {
        startButton.interactable = false;
        howToPlayButton.interactable = false;
        StartCoroutine(ShowHowToPlay());
    }

    public void OnExitHowToPlay()
    {
        StartCoroutine(HideHowToPlay());
    }

    IEnumerator ShowHowToPlay()
    {
        yield return FadeGroup(menuButtonsGroup, 0f, uiFadeDuration);

        howToPlayGroup.interactable = true;
        howToPlayGroup.blocksRaycasts = true;
        yield return FadeGroup(howToPlayGroup, 1f, uiFadeDuration);
    }

    IEnumerator HideHowToPlay()
    {
        howToPlayGroup.interactable = false;
        howToPlayGroup.blocksRaycasts = false;
        yield return FadeGroup(howToPlayGroup, 0f, uiFadeDuration);

        yield return FadeGroup(menuButtonsGroup, 1f, uiFadeDuration);
        startButton.interactable = true;
        howToPlayButton.interactable = true;
    }

    IEnumerator StartSequence()
    {
        yield return FadeGroup(menuButtonsGroup, 0f, uiFadeDuration);

        float startSpeed = background.scrollSpeed;
        float targetSpeed = startSpeed * speedUpMultiplier;
        float startPitch = carEngineAudio != null ? carEngineAudio.pitch : 1f;
        float targetPitch = startPitch * 1.4f;

        float t = 0f;
        while (t < speedUpDuration)
        {
            t += Time.deltaTime;
            float p = t / speedUpDuration;
            background.scrollSpeed = Mathf.Lerp(startSpeed, targetSpeed, p);
            if (carEngineAudio != null) carEngineAudio.pitch = Mathf.Lerp(startPitch, targetPitch, p);
            yield return null;
        }
        background.scrollSpeed = targetSpeed;

        yield return new WaitForSeconds(holdBeforeFadeDuration);

        fade.FadeOut();
        yield return new WaitForSeconds(fade.fadeDuration);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeGroup(CanvasGroup group, float target, float duration)
    {
        float start = group.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        group.alpha = target;
    }
}
