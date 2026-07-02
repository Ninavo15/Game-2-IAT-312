using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CarMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lateralSpeed;
    public float accel = 5f;
    public bool horizontalForward = false;

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
    private void FixedUpdate()
    {
        if (!stopped && horizontalForward && rb.position.x >= stopAtX)
        {
            stopped = true;
            if (lowFuelUI != null) lowFuelUI.SetActive(true);
        }

        Vector2 targetInput = stopped ? Vector2.zero : rawInput;

        //Debug.Log(smoothedInput);
        if (smoothedInput.y > 0.1f) 
        {
            lateralSpeed = 3f;
        } else {
            lateralSpeed = 0f;
        }
        smoothedInput = Vector2.Lerp(smoothedInput, targetInput, accel * Time.fixedDeltaTime);
        Vector2 nextVec = horizontalForward
            ? new Vector2(smoothedInput.y * speed, smoothedInput.x * lateralSpeed) * Time.fixedDeltaTime
            : new Vector2(smoothedInput.x * lateralSpeed, smoothedInput.y * speed) * Time.fixedDeltaTime;

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
        float throttle = Mathf.Clamp01(smoothedInput.y);

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
    }

    void OnInteract(InputValue value)
    {
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
