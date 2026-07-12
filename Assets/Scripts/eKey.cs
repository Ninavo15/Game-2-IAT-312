using UnityEngine;

public class eKey : MonoBehaviour
{
    [SerializeField] float speed;

    private void FixedUpdate()
    {
        transform.Translate(Vector2.left * speed * Time.fixedDeltaTime);
    }
}
