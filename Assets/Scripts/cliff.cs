using System.Collections;
using UnityEngine;

public class cliff : MonoBehaviour
{
    public float speed = 5;
    public Fading fade;
    AudioSource audios;
    public AudioClip skid;
    public GameObject ending;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audios = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed *  Time.deltaTime);        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Camera.main.transform.SetParent(transform, true);
        if (collision.CompareTag("Last Road"))
        {
            Camera.main.transform.SetParent(null);
            fade.FadeOut();
            audios.PlayOneShot(skid);
            StartCoroutine(EndingSequence());
        }
    }

    IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(3f);
        ending.SetActive(true);

    }
}
