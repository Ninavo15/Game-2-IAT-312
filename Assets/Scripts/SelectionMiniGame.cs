using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Runs the convenience store's item-selection mini-game: plays the "only
// enough money for one" intro line, then reveals the three buy buttons.
// Picking one records that choice in GlobalStore and fades back to Scene 4.
public class SelectionMiniGame : MonoBehaviour
{
    public DialogueController dialogue;
    public Fading fade;
    public GameObject[] choiceButtons;

    public string introLine = "Oh no ! I just have 5 dollars left. I cannot buy all. I have to choose one between these three";
    public float introDelay = 0.5f;
    public float introHoldDuration = 3f;

    public string nextSceneName = "Scene 4";

    [Header("Choose Sound")]
    public AudioSource chooseAudio;
    public AudioClip chooseClip;

    bool choiceMade;

    void Start()
    {
        SetButtonsActive(false);
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(introDelay);

        float revealTime = introLine.Length / Mathf.Max(dialogue.charactersPerSecond, 1f);
        dialogue.ShowLine(introLine, introHoldDuration);
        yield return new WaitForSeconds(revealTime + introHoldDuration);

        SetButtonsActive(true);
    }

    void SetButtonsActive(bool active)
    {
        foreach (GameObject button in choiceButtons)
        {
            if (button != null) button.SetActive(active);
        }
    }

    // Wired to the bleach button's OnClick.
    public void ChooseBleach()
    {
        if (choiceMade) return;
        choiceMade = true;
        GlobalStore.bleachPick = true;
        PlayChooseSound();
        StartCoroutine(FadeAndLoad());
    }

    // Wired to the scissors button's OnClick.
    public void ChooseScissors()
    {
        if (choiceMade) return;
        choiceMade = true;
        GlobalStore.scissorPick = true;
        PlayChooseSound();
        StartCoroutine(FadeAndLoad());
    }

    // Wired to the wrench button's OnClick.
    public void ChooseWrench()
    {
        if (choiceMade) return;
        choiceMade = true;
        GlobalStore.wrenchPick = true;
        PlayChooseSound();
        StartCoroutine(FadeAndLoad());
    }

    void PlayChooseSound()
    {
        if (chooseAudio != null && chooseClip != null) chooseAudio.PlayOneShot(chooseClip);
    }

    IEnumerator FadeAndLoad()
    {
        SetButtonsActive(false);
        fade.FadeOut();
        yield return new WaitForSeconds(fade.fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
