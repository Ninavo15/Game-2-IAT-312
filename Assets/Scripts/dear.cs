using UnityEngine;

public class dear : MonoBehaviour
{
    public float speed = 5f;
    public GameObject Dear;
    private bool triggered = false; // prevents re-triggering

    private void Update()
    {
        if (triggered)
        {
            Dear.transform.Translate(Vector2.left * speed * Time.deltaTime);

        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggered = true;
            Dear.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("hello");
            Dear.SetActive(false);
        }
    }
}
