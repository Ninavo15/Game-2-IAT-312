using UnityEngine;

public class road : MonoBehaviour
{
    private float chunkHeight;
    private Transform otherChunk;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chunkHeight = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public void SetOtherChunk(Transform other)
    {
        otherChunk = other;
    }

    void Update()
    {
        
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // player has left this chunk - move it above the other chunk
            transform.position = new Vector3(
                transform.position.x,
                otherChunk.position.y + chunkHeight,
                0
            );
        }
    }
}
