using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public road chunk1;
    public road2 chunk2;



    void Start()
    {

        chunk1.SetOtherChunk(chunk2.transform);
        chunk2.SetOtherChunk(chunk1.transform);

    }

}