using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    public int speed = 5;
    public float lateralSpeed = 2f;

    public Vector2 inputVec;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }
    private void FixedUpdate()
    {
        Vector2 nextVec = new Vector2(
            inputVec.x * lateralSpeed,   // side to side
            inputVec.y * speed            // forward / back
        ) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

}
