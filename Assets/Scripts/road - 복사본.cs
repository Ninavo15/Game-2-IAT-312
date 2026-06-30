using UnityEngine;

public class road2 : MonoBehaviour
{
    private float chunkHeight;
    private Transform otherChunk;
    public float chunkCounter = 0;
    

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
        Debug.Log(chunkCounter);
        if (other.CompareTag("Player") && chunkCounter < 4)
        {
            // player has left this chunk - move it above the other chunk

            chunkCounter++;
            transform.position = new Vector3(transform.position.x, otherChunk.position.y + chunkHeight, 0);
        }
    }
}
