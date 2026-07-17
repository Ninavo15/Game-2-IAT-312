using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoliceDialogue : MonoBehaviour
{

    public GameObject d1;
    public GameObject d2;
    public GameObject d3;
    public GameObject d4;

    public GameObject final;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DialogueSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DialogueSequence()
    {
        yield return new WaitForSeconds(2f);
        d1.SetActive(true);
        yield return new WaitForSeconds(3f);
        d2.SetActive(true);
        yield return new WaitForSeconds(5f);
        d3.SetActive(true);
        yield return new WaitForSeconds(3f);
        d4.SetActive(true);
        yield return new WaitForSeconds(3f);
        d1.SetActive(false);
        d2.SetActive(false);
        d3.SetActive(false);
        d4.SetActive(false);

        final.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Home");
    }
}
