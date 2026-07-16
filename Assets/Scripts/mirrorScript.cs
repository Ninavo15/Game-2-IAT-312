using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class mirrorScript : MonoBehaviour
{
    public Text countdownText;
    public Image bleach;
    public Image scissor;
    public Image wrench;
    public float countdownFrom = 10f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (StressSystem.stressPoints == 1)
        {
            countdownFrom = 9f;
        }
        else if (StressSystem.stressPoints == 2)
        {
            countdownFrom = 7f;
        }
        else if (StressSystem.stressPoints == 3)
        {
            countdownFrom = 5f;
        }
        else if (StressSystem.stressPoints == 3)
        {
            countdownFrom = 4f;
        }
        else if (StressSystem.stressPoints == 4)
        {
            countdownFrom = 3f;
        }
        else if (StressSystem.stressPoints == 5)
        {
            countdownFrom = 2f;
        }

        if (GlobalStore.bleachPick)
        {
            Color sc = scissor.color;
            Color wr = wrench.color;
            sc.a = 0.3f;
            wr.a = 0.3f;
            scissor.color = sc;
            wrench.color = wr;
        }
        if (GlobalStore.scissorPick)
        {
            Color bl = scissor.color;
            Color wr = wrench.color;
            bl.a = 0.3f;
            wr.a = 0.3f;
            bleach.color = bl;
            wrench.color = wr;
        }
        if (GlobalStore.wrenchPick)
        {
            Color bl = scissor.color;
            Color sc = scissor.color;
            bl.a = 0.3f;
            sc.a = 0.3f;

            bleach.color = bl;
            scissor.color = sc;
        }
        StartCoroutine(RunCountdown());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RunCountdown()
    {
        float t = countdownFrom;
        while (t > 0)
        {
            countdownText.text = Mathf.CeilToInt(t).ToString();
            yield return new WaitForSeconds(1f);
            t--;
        }
        countdownText.text = "TOO LATE";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        // start gameplay here
    }



}
