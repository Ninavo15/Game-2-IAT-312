using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class promptPop : MonoBehaviour
{
    [Header("References")]
    public GameObject choicePanel;
    public GameObject promptText1;
    public GameObject promptText2;
    public GameObject promptText3;
    public GameObject promptText4;
    public GameObject promptText5;
    public GameObject promptText6;

    public float mapDisplayDuration = 2.0f;
    public void ShowPrompt(GameObject prompt)
    {
        prompt.SetActive(true);
        Debug.Log("prompt shown");
        StartCoroutine(RevealPromptAfterDelay(prompt));
    }

    IEnumerator RevealPromptAfterDelay(GameObject prompt)
    {
        yield return new WaitForSeconds(mapDisplayDuration);
        prompt.SetActive(false);
    }
}
