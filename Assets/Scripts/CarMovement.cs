using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lateralSpeed;
    public float accel = 5f;


    private Vector2 rawInput;
    private Vector2 smoothedInput;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }
    private void FixedUpdate()
    {
        //Debug.Log(smoothedInput);
        if (smoothedInput.y > 0.1f) 
        {
            lateralSpeed = 3f;
        } else {
            lateralSpeed = 0f;
        }
            smoothedInput = Vector2.Lerp(smoothedInput, rawInput, accel * Time.fixedDeltaTime);
        Vector2 nextVec = new Vector2(smoothedInput.x * lateralSpeed, smoothedInput.y * speed) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

}
