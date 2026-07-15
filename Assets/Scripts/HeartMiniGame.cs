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

    private bool jumpStarted = false;
    
    bool keyIn;

    [Header("Parameters")]
    public float life = 2;
    public float JumpSpeed = 3;
    public List<GameObject> ekeys = new List<GameObject>();

    private bool jumping = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        ghostRb = ghost.GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E) && life > 0 && ekeys.Count > 0)
            {
                Destroy(ekeys[0]);
                ekeys.RemoveAt(0);
            }
        }
        if (!keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                vignetteEffect.TriggerVignette();
                audioSource.PlayOneShot(gasp);
                audioSource.PlayOneShot(stab);
                life--;
                Debug.Log("Your life remaining: " + life);
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

}
