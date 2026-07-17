using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class mirrorScript : MonoBehaviour
{


    [Header("UI")]
    public Text countdownText;
    public Image bleach;
    public Image scissor;
    public Image wrench;
    public Button bleachButton;
    public Button scissorButton;
    public Button wrenchButton;
    public GameObject bB;
    public GameObject sB;
    public GameObject wB;


    public float countdownFrom = 10f;
    public GameObject blink;
    public GameObject timerText;
    public GameObject objOptions;
    public GameObject scissored;
    public GameObject bleached;



    [Header("Audios")]
    public AudioClip policeWalk;
    public AudioClip glassShatter;
    public AudioClip bleachSplash;
    public AudioClip scissorRip;
    public AudioClip policeSiren;

    AudioSource audioSource;
    Animator animator;
    promptPop pp;
    public float cutsceneTime = 25f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        pp = GetComponent<promptPop>();
        audioSource = GetComponent<AudioSource>();

        bleachButton.onClick.AddListener(() => OnItemClicked("bleach"));
        scissorButton.onClick.AddListener(() => OnItemClicked("scissor"));
        wrenchButton.onClick.AddListener(() => OnItemClicked("wrench"));


        if (StressSystem.stressPoints <= 1)
        {
            countdownFrom = 10f;
        }
        else if (StressSystem.stressPoints == 2)
        {
            countdownFrom = 7f;
        }
        else if (StressSystem.stressPoints == 3)
        {
            countdownFrom = 5f;
        }
        else if (StressSystem.stressPoints == 4)
        {
            countdownFrom = 4f;
        }


        if (GlobalStore.bleachPick)
        {
            Color sc = scissor.color;
            Color wr = wrench.color;
            sc.a = 0.3f;
            wr.a = 0.3f;
            scissor.color = sc;
            wrench.color = wr;
            bB.SetActive(true);

        }
        if (GlobalStore.scissorPick)
        {
            Color bl = scissor.color;
            Color wr = wrench.color;
            bl.a = 0.3f;
            wr.a = 0.3f;
            bleach.color = bl;
            wrench.color = wr;
            sB.SetActive(true);

        }
        if (GlobalStore.wrenchPick)
        {
            Color bl = scissor.color;
            Color sc = scissor.color;
            bl.a = 0.3f;
            sc.a = 0.3f;

            bleach.color = bl;
            scissor.color = sc;
            wB.SetActive(true);

        }
        StartCoroutine(startScare());
        Debug.Log(StressSystem.stressPoints);
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
        if (StressSystem.stressPoints >= 2)
        {
            animator.SetBool("Stressed", true);
            yield return new WaitForSeconds(2.5f);
            blink.SetActive(true);
            animator.SetBool("Stressed", false);
            yield return new WaitForSeconds(1.4f);
            pp.ShowPrompt(pp.promptText2);
            yield return new WaitForSeconds(3f);

        }
        pp.ShowPrompt(pp.promptText3);
        audioSource.PlayOneShot(policeSiren);
        yield return new WaitForSeconds(cutsceneTime);
        pp.ShowPrompt(pp.promptText4); // i need to get rid of blood
        timerText.SetActive(true);
        objOptions.SetActive(true);
        StartCoroutine(RunCountdown());

        audioSource.PlayOneShot(policeWalk);

    }
    void OnItemClicked(string item)
    {
        bleachButton.interactable = false;
        scissorButton.interactable = false;
        wrenchButton.interactable = false;

        AudioClip clipToPlay = GetClipForItem(item);
        StartCoroutine(PlaySoundThenLoadScene(clipToPlay, GetSceneForItem(item)));
    }
    AudioClip GetClipForItem(string item)
    {
        switch (item)
        {
            case "bleach": return bleachSplash;
            case "scissor": return scissorRip;
            case "wrench": return glassShatter; 
            default: return null;
        }
    }
    IEnumerator PlaySoundThenLoadScene(AudioClip clip, string sceneName)
    {
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        objOptions.SetActive(false);
        if (GlobalStore.scissorPick)
        {
            scissored.SetActive(true);
            yield return new WaitForSeconds(1);
            pp.ShowPrompt(pp.promptText5); 
        }
        if (GlobalStore.bleachPick)
        {
            bleached.SetActive(true);
            yield return new WaitForSeconds(1);
            pp.ShowPrompt(pp.promptText5); // Now I can go talk to the police

        }
        SceneManager.LoadScene(sceneName);
    }
    string GetSceneForItem(string item)
    {
        switch (item)
        {
            case "bleach": return "BleachEnding";
            case "scissor": return "ScissorEnding";
            case "wrench": return "WrenchEnding";
            default: return "";
        }
    }
}