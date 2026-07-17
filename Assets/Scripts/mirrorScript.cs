using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class mirrorScript : MonoBehaviour
{


    [Header("UI")]
    public Text countdownText;
    public Image bleach;
    public Image scissor;
    public Image wrench;
    public float countdownFrom = 10f;
    public GameObject blink;
    public GameObject objOptions;

    [Header("Audios")]
    public AudioClip policeWalk;
    public AudioClip glassShatter;
    public AudioClip bleachSplash;
    public AudioClip scissorRip;
    public AudioClip policeSiren;

    AudioSource audioSource;
    Animator animator;
    promptPop pp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        pp = GetComponent<promptPop>();
        audioSource = GetComponent<AudioSource>();


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
        StartCoroutine(startScare());
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

    }
    IEnumerator startScare()
    {
        pp.ShowPrompt(pp.promptText1);
        yield return new WaitForSeconds(3f);
        if (StressSystem.stressPoints >= 0)
        {
            animator.SetBool("Stressed", true);
            yield return new WaitForSeconds(2.5f);
            blink.SetActive(true);
            animator.SetBool("Stressed", false);
            yield return new WaitForSeconds(1.4f);
            pp.ShowPrompt(pp.promptText2);
        }
        audioSource.PlayOneShot(policeSiren);
        pp.ShowPrompt(pp.promptText3);

        
    }



}
