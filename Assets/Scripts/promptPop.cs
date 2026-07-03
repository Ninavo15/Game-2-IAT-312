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
    public void ShowPrompt()
    {
        choicePanel.SetActive(true);
        

        
    
        promptText1.SetActive(true);
        

        StartCoroutine(RevealPromptAfterDelay());
    }

    IEnumerator RevealPromptAfterDelay()
    {
        yield return new WaitForSeconds(mapDisplayDuration);

        promptText1.SetActive(false);
        promptText2.SetActive(true);

        yield return new WaitForSeconds(mapDisplayDuration);

        promptText2.SetActive(false);

        

    }
}
