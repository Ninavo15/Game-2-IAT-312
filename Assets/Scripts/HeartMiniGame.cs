using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HeartMiniGame : MonoBehaviour
{
    bool keyIn;
    public float life = 3;
    public List<GameObject> ekeys = new List<GameObject>();

    private void Update()
    {
        if (keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E) && life > 0 && ekeys.Count > 0)
            {
                Destroy(ekeys[0]);
                ekeys.RemoveAt(0);
            }
        }
        if (!keyIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                life--;
                Debug.Log("Your life remaining: " + life);
            }
        }
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
