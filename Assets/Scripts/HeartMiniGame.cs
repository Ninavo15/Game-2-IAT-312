using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartMiniGame : MonoBehaviour
{
    [SerializeField]
    GameObject ghost;
    [SerializeField]
    jumpscare jump;
    [SerializeField]
    GameObject ekeyy;
    [SerializeField]
    GameObject gPic;
    SpriteRenderer sr;

    bool keyIn;
    public float life = 3;
    public float JumpSpeed = 3;
    public List<GameObject> ekeys = new List<GameObject>();
    private bool jumping = false;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


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
        if (life <= 0 && !jump.jumped)
        {
            sr.enabled = false;
            gPic.SetActive(false);
            Destroy(ekeyy);
            StartCoroutine(JumpSequence());
            if (jumping)
            {
                ghost.transform.Translate(Vector2.up * JumpSpeed * Time.deltaTime);
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

    IEnumerator JumpSequence()
    {
        yield return new WaitForSeconds(4);
        jumping = true;
    }

}
