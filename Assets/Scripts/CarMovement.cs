using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CarMovement : MonoBehaviour
{

    [Header("Intro Sequence")]
    public bool introActive;
    public float introDuration = 1f; // how long car drives itself in

    [Header("Auto Drive To Position")]
    public bool autoDriveToPosition = false;
    public Vector2 autoDriveTarget;
    public float autoDriveStopDistance = 0.05f;

    public float speed = 5f;
    public float lateralSpeed;
    public float accel = 5f;
    public bool horizontalForward = false;
    public bool lockLateralMovement = false;
    //left/right input directly drives left/right screen movement
    public bool directLeftRight = false;

    [Header("Stop & Exit")]
    public float stopAtX = 5.6f;
    public GameObject lowFuelUI;
    public GameObject player;
    public Vector2 exitOffset = new Vector2(-6.5858803f, -2.5f);

    [Header("Engine Sound")]
    public AudioSource engineSound;
    public AudioSource beerSound;
    public AudioClip engineClip;
    public AudioClip beerClip;
    public AudioClip crashClip;
    public AudioSource carCrash;

    public Light2D spotLight;


    public float minVolume = 0.1f;
    public float maxVolume = 1f;
    public float minPitch = 0.8f;
    public float maxPitch = 1.6f;



    public Fading fade;
    public bool stopped = false;
    private Vector2 rawInput;
    private Vector2 smoothedInput;
    Rigidbody2D rb;
    RectTransform rectTransform;

    public Transform cameraTransform;
    public string sceneName;
    private bool crashed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();

        engineSound.clip = engineClip;
        beerSound.clip = beerClip;
        engineSound.loop = true;
        beerSound.loop = true;

        engineSound.volume = minVolume;
        beerSound.volume = minVolume;
        engineSound.Play();
        beerSound.Play();
    }
    private void Start()
    {
        if (introActive)
        {
            stopped = true; // block player input
            StartCoroutine(IntroSequence());
        }
        else if (autoDriveToPosition)
        {
            stopped = true; // block player input - this scene drives itself, only E to exit works
            StartCoroutine(AutoDriveSequence());
        }
    }
    IEnumerator IntroSequence()
    {
        float elapsed = 0f;
        while (elapsed < introDuration)
        {
            elapsed += Time.fixedDeltaTime;

            if (horizontalForward)
            {
                rb.MovePosition(rb.position + Vector2.right * speed * Time.fixedDeltaTime);
                if (elapsed >= introDuration)
                {
                    engineSound.Pause();
                    beerSound.Pause();
                    stopped = true;
                    spotLight.enabled = false;
                }
            }
            else
            {
                rb.MovePosition(rb.position + Vector2.up * speed * Time.fixedDeltaTime);
            }
            yield return new WaitForFixedUpdate();
        }
        if (!horizontalForward)
        {
            stopped = false;
        }
        introActive = false;

    }
    IEnumerator AutoDriveSequence()
    {
        while (Vector2.Distance(rb.position, autoDriveTarget) > autoDriveStopDistance)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, autoDriveTarget, speed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(autoDriveTarget);
        if (lowFuelUI != null) lowFuelUI.SetActive(true);
        // stays stopped - the car never responds to player input in this scene, only E to exit
    }

    private void FixedUpdate()
    {
        // Auto-drive and intro sequences block player input
        if (introActive || autoDriveToPosition) return;
        if (!stopped && (horizontalForward || directLeftRight) && rb.position.x >= stopAtX)
        {
            stopped = true;
            if (lowFuelUI != null) lowFuelUI.SetActive(true);
        }

        Vector2 targetInput = stopped ? Vector2.zero : rawInput;

        Vector2 nextVec;
        if (directLeftRight)
        {
            smoothedInput = Vector2.Lerp(smoothedInput, targetInput, accel * Time.fixedDeltaTime);
            nextVec = new Vector2(smoothedInput.x * speed, 0f) * Time.fixedDeltaTime;
        }
        else
        {
            //Debug.Log(smoothedInput);
            if (smoothedInput.y > 0.1f)
            {
                lateralSpeed = 3f;
            } else {
                lateralSpeed = 0f;
            }
            smoothedInput = Vector2.Lerp(smoothedInput, targetInput, accel * Time.fixedDeltaTime);
            nextVec = horizontalForward
                ? new Vector2(smoothedInput.y * speed, smoothedInput.x * lateralSpeed) * Time.fixedDeltaTime
                : new Vector2(smoothedInput.x * lateralSpeed, smoothedInput.y * speed) * Time.fixedDeltaTime;
        }

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += nextVec;
        }
        else
        {
            rb.MovePosition(rb.position + nextVec);
        }
        UpdateEngineSound();

    }
    void UpdateEngineSound()
    {
        float throttle = directLeftRight ? Mathf.Abs(smoothedInput.x) : Mathf.Clamp01(smoothedInput.y);

        engineSound.volume = Mathf.Lerp(minVolume, maxVolume, throttle);
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, throttle);
        beerSound.volume = Mathf.Lerp(minVolume, maxVolume, throttle);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Last Road"))
        {
            cameraTransform.SetParent(null);
            StartCoroutine(FadeAndLoad(sceneName));
        }
        if (collision.CompareTag("Cam Road"))
        {
            Debug.Log("Cam Road triggered");
            cameraTransform.SetParent(transform, true);
            Debug.Log("Camera parent is now: " + cameraTransform.parent.name);
        }
        if(collision.CompareTag("Ending Road"))
        {
            cameraTransform.SetParent(null);
            StartCoroutine(FadeAndLoad("Ending 2 (DIE)"));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("log") && !crashed)
        {
            stopped = true;
            crashed = true;
            carCrash.Play();
            StartCoroutine(FadeAndLoad("Ending 1 (CAPTURE)"));
        }
    }
    IEnumerator FadeAndLoad(string sceneName)
    {
        fade.FadeOut();
        yield return new WaitForSeconds(fade.fadeDuration);
        SceneManager.LoadScene(sceneName);
    }
    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
        
        if (lockLateralMovement) rawInput.x = 0f;
        
    }

    void OnInteract(InputValue value)
    {
        Debug.Log("OnInteract fired!");
        
        if (!stopped || player == null) return;

        engineSound.Stop();
        beerSound.Stop();
        GetComponent<PlayerInput>().enabled = false;
        enabled = false;

        player.transform.position = (Vector2)transform.position + exitOffset;
        player.SetActive(true);

        if (lowFuelUI != null) lowFuelUI.SetActive(false);
    }

}
