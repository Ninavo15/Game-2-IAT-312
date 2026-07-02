using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CarMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lateralSpeed;
    public float accel = 5f;

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

    public Transform cameraTransform;
    public string sceneName;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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

        Vector2 targetInput = stopped ? Vector2.zero : rawInput;

        //Debug.Log(smoothedInput);
        if (smoothedInput.y > 0.1f) 
        {
            lateralSpeed = 3f;
        } else {
            lateralSpeed = 0f;
        }
        smoothedInput = Vector2.Lerp(smoothedInput, targetInput, accel * Time.fixedDeltaTime);
        Vector2 nextVec = new Vector2(smoothedInput.x * lateralSpeed, smoothedInput.y * speed) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
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

}
