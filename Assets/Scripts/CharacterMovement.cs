using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 3f;

    public SpriteRenderer spriteRenderer;
    public Sprite sideSprite;
    public Sprite frontSprite;
    public Sprite backSprite;
    public GameObject dialogue;
    public GameObject washroomDialogue;

    public bool motelEnter = false;
    private bool washroom = false;

    // True while facing right (the sideSprite's default, unflipped orientation).
    // Persists across sprite swaps so other scripts can read the last real
    // facing direction even while showing a different sprite (e.g. hand up).
    public bool FacingRight { get; private set; } = true;

    Rigidbody2D rb;
    Vector2 rawInput;
    private bool mirror = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rawInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        // Freeze movement while a dialogue line or scene fade is playing.
        Vector2 moveInput = (DialogueController.IsActive || Fading.IsFading) ? Vector2.zero : rawInput;

        Vector2 nextPos = rb.position + new Vector2(moveInput.x, 0f) * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
        UpdateSprite(moveInput);
    }
    private void Update()
    {
        if (motelEnter)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("Scene 5");
                Debug.Log("scene 5");

<<<<<<< Updated upstream
            }
        }
        if (washroom)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("wash");
                transform.position = new Vector2(2.94f, -1.34f);
            }
        }
    }
    void UpdateSprite()
    {

        if (rawInput.x == 0f) return;
=======
    void UpdateSprite(Vector2 moveInput)
    {
        if (moveInput.x == 0f) return;
>>>>>>> Stashed changes

        FacingRight = moveInput.x > 0f;
        spriteRenderer.sprite = sideSprite;
        spriteRenderer.flipX = !FacingRight;
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("motel"))
        {
            dialogue.SetActive(true);
            motelEnter = true;
        }
        if(collision.CompareTag("washroom"))
        {
            dialogue.SetActive(true);
            washroom = true;
        }
        if (collision.CompareTag("mirror"))
        {
            washroomDialogue.SetActive(true);
            mirror = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("motel"))
        {
            dialogue.SetActive(false);
            motelEnter = false;


        }
        if (collision.CompareTag("washroom"))
        {
            dialogue.SetActive(false);
            washroom = false;
        }
        if (collision.CompareTag("mirror"))
        {
            washroomDialogue.SetActive(false);
            mirror = false;
        }

    }
}
