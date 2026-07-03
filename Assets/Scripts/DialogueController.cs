using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public float displayDuration = 2.5f;

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
        dialogueText.text = line;
        dialoguePanel.SetActive(true);
        yield return new WaitForSeconds(duration);
        dialoguePanel.SetActive(false);
    }
}
