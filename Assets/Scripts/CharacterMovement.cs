using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 3f;
    public float maxY = 1.5f;

    public SpriteRenderer spriteRenderer;
    public Sprite sideSprite;
    public Sprite frontSprite;
    public Sprite backSprite;

    Rigidbody2D rb;
    Vector2 rawInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 nextPos = rb.position + rawInput * speed * Time.fixedDeltaTime;
        nextPos.y = Mathf.Min(nextPos.y, maxY);
        rb.MovePosition(nextPos);
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (rawInput == Vector2.zero) return;

        if (Mathf.Abs(rawInput.x) >= Mathf.Abs(rawInput.y))
        {
            spriteRenderer.sprite = sideSprite;
            spriteRenderer.flipX = rawInput.x < 0f;
        }
        else if (rawInput.y > 0f)
        {
            spriteRenderer.sprite = backSprite;
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.sprite = frontSprite;
            spriteRenderer.flipX = false;
        }
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }
}
