using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public Transform[] segments;
    public float scrollSpeed = 1.5f;

    float segmentWidth;

    void Start()
    {
        segmentWidth = segments[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float delta = scrollSpeed * Time.deltaTime;
        foreach (var seg in segments)
            seg.position += Vector3.left * delta;

        foreach (var seg in segments)
        {
            if (seg.position.x <= -segmentWidth)
            {
                float rightmostX = segments[0].position.x;
                foreach (var other in segments)
                    if (other.position.x > rightmostX) rightmostX = other.position.x;
                seg.position = new Vector3(rightmostX + segmentWidth, seg.position.y, seg.position.z);
            }
        }
    }
}

