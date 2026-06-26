using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    public Transform[] roadChunks; // assign 2 road segment objects
    public float scrollSpeed = 5f;
    private float chunkHeight;

    void Start()
    {
        // get the height of one road chunk in world units
        chunkHeight = roadChunks[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        foreach (Transform chunk in roadChunks)
        {
            // move each chunk downward
            chunk.position += Vector3.down * scrollSpeed * Time.deltaTime;

            // if chunk has scrolled fully below the camera, wrap it to the top
            if (chunk.position.y < -chunkHeight)
            {
                chunk.position += Vector3.up * chunkHeight * roadChunks.Length;
            }
        }
    }
}