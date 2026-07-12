using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HeartMiniGame : MonoBehaviour
{
    bool keyIn;
    public List<GameObject> ekeys = new List<GameObject>();

    private void Update()
    {
        if (keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E");
                ekeys.RemoveAt(0);
            }
        }
        Debug.Log(keyIn);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ekeys")) { 
            keyIn = true;
            ekeys.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ekeys"))
        {
            keyIn = false;
            ekeys.Remove(collision.gameObject);
        }
    }
}
