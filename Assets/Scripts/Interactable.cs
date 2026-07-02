using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public GameObject promptText;
    public UnityEvent onInteract;

    bool playerInRange = false;

    void Awake()
    {
        if (promptText != null) promptText.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = true;
        if (promptText != null) promptText.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = false;
        if (promptText != null) promptText.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            onInteract.Invoke();
        }
    }
}
