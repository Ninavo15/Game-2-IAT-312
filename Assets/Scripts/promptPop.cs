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

    public float mapDisplayDuration = 2.0f;
    public void ShowPrompt1()
    {
        choicePanel.SetActive(true);
        promptText1.SetActive(true);
        StartCoroutine(RevealPrompt1AfterDelay());

    }
    public void ShowPrompt2()
    {
        promptText2.SetActive(true);
        StartCoroutine(RevealPrompt2AfterDelay());

    }
    IEnumerator RevealPrompt1AfterDelay()
    {
        yield return new WaitForSeconds(mapDisplayDuration);
        promptText1.SetActive(false);

    }
    IEnumerator RevealPrompt2AfterDelay()
    {
        yield return new WaitForSeconds(mapDisplayDuration);
        promptText2.SetActive(false);


    }
}
