using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HeartMiniGame : MonoBehaviour
{





    [Header("GameObject")]
    [SerializeField]
    GameObject ghost;
    [SerializeField]
    GameObject ekeyy;
    [SerializeField]
    GameObject gPic;
    [SerializeField]
    GameObject blink;
    [SerializeField]
    GameObject carPov;
    [SerializeField]
    GameObject car;
    [SerializeField]
    GameObject lastE;



    [Header("Audio")]

    [SerializeField] 
    AudioSource audioSource;
    [SerializeField]
    AudioSource jumpscare;
    [SerializeField] 
    AudioClip gasp;
    [SerializeField] 
    AudioClip stab;

    [Header("Etc.")]
    [SerializeField]
    jumpscare jump;
    [SerializeField]
    Rigidbody2D ghostRb;
    [SerializeField] 
    promptPop pp;
    [SerializeField] CarMovement carMove;


    SpriteRenderer sr;
    public VignetteEffect vignetteEffect;
    private GameObject collectedKey;
    private bool jumpStarted = false;
    
    bool keyIn;

    [Header("Heart Animation")]
    public float beatSize = 1.15f;
    public float beatSpeed = 1f;
    Vector3 originalScale;

    [Header("Parameters")]
    public float life = 2;
    float ogLife;


    public float JumpSpeed = 3;
    public List<GameObject> ekeys = new List<GameObject>();

    private bool jumping = false;

    private void Start()
    {
        ogLife = life;
        sr = GetComponent<SpriteRenderer>();
        ghostRb = ghost.GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        StartCoroutine(Beat());
    }


    private void Update()
    {

        if (life != ogLife)
        {
            vignetteEffect.TriggerVignette();
            audioSource.PlayOneShot(gasp);
            audioSource.PlayOneShot(stab);
            ogLife--;
            Debug.Log("Your life remaining: " + life);
        }


        if (keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E) && life > 0 && ekeys.Count > 0)
            {
                collectedKey = ekeys[0];
                
                GameObject key = ekeys[0];
                if (key == lastE)
                {
                    blink.SetActive(true);
                    StartCoroutine(SafeSequence());
                    sr.enabled = false;
                    Destroy(ekeyy);
                }
                Destroy(key);
                ekeys.RemoveAt(0);
            }
        }


        if (!keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                life--;
            }
        }
        if (life <= 0 && !jump.jumped)
        {
            blink.SetActive(true); 
            sr.enabled = false; 
            Destroy(ekeyy);

            if (!jumpStarted)
            {
                jumpStarted = true;
                StartCoroutine(JumpSequence());
            }

            if (jumping)
            {
                ghostRb.MovePosition(ghostRb.position + Vector2.left * JumpSpeed * Time.deltaTime);
            }
            StressSystem.AddPoint();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ekeys")) { 
            keyIn = true;
            ekeys.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ekeys"))
        {
            keyIn = false;

            if (collision.gameObject != collectedKey)
            {
                life--;
            }

            ekeys.Remove(collision.gameObject);
        }
    }

    IEnumerator JumpSequence()
    {
        yield return new WaitForSeconds(1);
        gPic.SetActive(false);
        yield return new WaitForSeconds(4);
        jumpscare.Play();
        jumping = true;
        yield return new WaitForSeconds(2.5f);
        ghost.SetActive(false);
        carPov.SetActive(false);
        car.SetActive(true);
        pp.ShowPrompt2();
        carMove.stopped = false;    
    }
    IEnumerator SafeSequence()
    {
        yield return new WaitForSeconds(1);
        gPic.SetActive(false);
        yield return new WaitForSeconds(2);
        ghost.SetActive(false);
        carPov.SetActive(false);
        car.SetActive(true);
        pp.ShowPrompt2();
        carMove.stopped = false;  
    }
    IEnumerator Beat()
    {
        while (true)
        {
            // First beat
            transform.localScale = originalScale * beatSize;
            yield return new WaitForSeconds(beatSpeed);

            transform.localScale = originalScale;
            yield return new WaitForSeconds(0.3f);

            // Second beat
            transform.localScale = originalScale * (beatSize * 0.95f);
            yield return new WaitForSeconds(beatSpeed);

            transform.localScale = originalScale;

            // Pause 
            yield return new WaitForSeconds(0.6f);
        }
    }
}
