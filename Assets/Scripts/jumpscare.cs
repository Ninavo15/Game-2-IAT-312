using UnityEngine;

public class jumpscare : MonoBehaviour
{
    public bool jumped = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ghost"))
        {

            jumped = true;
        }
    }
}
