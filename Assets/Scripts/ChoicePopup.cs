using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChoicePopup : MonoBehaviour
{
    [Header("References")]
    public GameObject choicePanel;
    public CarMovement car;

    [Header("Stage 1 - Map")]
    public GameObject mapImage;
    public GameObject mapPromptText; 
    public float mapDisplayDuration = 3f;

    [Header("Stage 2 - Choice")]
    public GameObject choiceUI; 
    public GameObject arrowA;
    public GameObject arrowB;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip beepSound;

    [Header("Scenes")]
    public string pathAScene;
    public string pathBScene;


    public Fading fade;
    private bool choiceActive = false;
    private int selectedPath = 0;

    public void ShowChoice()
    {
        choicePanel.SetActive(true);
        car.stopped = true;

        if (audioSource != null && beepSound != null)
        {
            audioSource.PlayOneShot(beepSound);

        }

        // show map, hide choice UI
        mapImage.SetActive(true);
        mapPromptText.SetActive(true);
        choiceUI.SetActive(false);
        arrowA.SetActive(false);
        arrowB.SetActive(false);

        StartCoroutine(RevealChoiceAfterDelay());
    }

    IEnumerator RevealChoiceAfterDelay()
    {
        yield return new WaitForSeconds(mapDisplayDuration);

        //  hide map, show choice UI
        mapImage.SetActive(false);
        mapPromptText.SetActive(false);
        choiceUI.SetActive(true);

        choiceActive = true;
        selectedPath = 0;
        UpdateArrows();

    }

    void Update()
    {
        if (!choiceActive) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            selectedPath = 1;
            UpdateArrows();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            selectedPath = 2;
            UpdateArrows();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && selectedPath != 0)
        {
            ConfirmChoice();
        }
    }

    void UpdateArrows()
    {
        arrowA.SetActive(selectedPath == 1);
        arrowB.SetActive(selectedPath == 2);
    }

    void ConfirmChoice()
    {
        choiceActive = false;

        if (selectedPath == 1)
            StartCoroutine(FadeAndLoad(pathAScene));
        else if (selectedPath == 2)
            StartCoroutine(FadeAndLoad(pathBScene));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        fade.FadeOut();
        yield return new WaitForSeconds(fade.fadeDuration);
        choicePanel.SetActive(false);           // disable AFTER fade finishes
        SceneManager.LoadScene(sceneName);
    }
}