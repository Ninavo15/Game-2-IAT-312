using UnityEngine;

public class FirstChoice : MonoBehaviour
{
    public CarMovement car;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Debug.Log("in");
            car.speed = 1;
            car.lateralSpeed = 1;
        }
    }
}
