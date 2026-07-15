using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static bool IsActive { get; private set; }

    public GameObject dialoguePanel;
    public Text dialogueText;
    public float displayDuration = 2.5f;
    public float charactersPerSecond = 30f;

    Coroutine current;

    public void ShowLine(string line)
    {
        ShowLine(line, displayDuration);
    }

    public void ShowLine(string line, float duration)
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(ShowLineRoutine(line, duration));
    }

    IEnumerator ShowLineRoutine(string line, float duration)
    {
        IsActive = true;
        dialoguePanel.SetActive(true);

        // Reveal the line left to right
        dialogueText.text = "";
        float delayPerChar = 1f / Mathf.Max(charactersPerSecond, 1f);
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(delayPerChar);
        }

        yield return new WaitForSeconds(duration);
        dialoguePanel.SetActive(false);
        IsActive = false;
    }
}
