using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ChoicePopup : MonoBehaviour
{
    [Header("References")]
    public GameObject choicePanel;
    public CarMovement car;

    [Header("Stage 1 - Map")]
    public GameObject mapImage;
    public GameObject mapPromptText; // "Why did the map stop working?"
    public float mapDisplayDuration = 3f;

    [Header("Stage 2 - Choice")]
    public GameObject choiceUI; // the Q/E/Space prompt group
    public GameObject arrowA;
    public GameObject arrowB;

    private bool choiceActive = false;
    private int selectedPath = 0;

    public void ShowChoice()
    {
        choicePanel.SetActive(true);
        car.stopped = true;

        // stage 1: show map, hide choice UI
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

        // stage 2: hide map, show choice UI
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
        choicePanel.SetActive(false);
        choiceActive = false;
        car.stopped = false;

        if (selectedPath == 1)
        {
            Debug.Log("Player chose Path A");
        }
        else if (selectedPath == 2)
        {
            Debug.Log("Player chose Path B");
        }
    }
}