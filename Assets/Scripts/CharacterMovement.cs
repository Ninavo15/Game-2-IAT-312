using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 3f;

    public SpriteRenderer spriteRenderer;
    public Sprite sideSprite;
    public Sprite frontSprite;
    public Sprite backSprite;

    // True while facing right (the sideSprite's default, unflipped orientation).
    // Persists across sprite swaps so other scripts can read the last real
    // facing direction even while showing a different sprite (e.g. hand up).
    public bool FacingRight { get; private set; } = true;

    Rigidbody2D rb;
    Vector2 rawInput;

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
        Vector2 nextPos = rb.position + new Vector2(rawInput.x, 0f) * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (rawInput.x == 0f) return;

        FacingRight = rawInput.x > 0f;
        spriteRenderer.sprite = sideSprite;
        spriteRenderer.flipX = !FacingRight;
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }
}
